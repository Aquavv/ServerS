using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ServerPickerX.Comparers;
using ServerPickerX.Models;
using ServerPickerX.Services.DependencyInjection;
using ServerPickerX.Services.Localizations;
using ServerPickerX.Services.Loggers;
using ServerPickerX.Services.MessageBoxes;
using ServerPickerX.Services.Servers;
using ServerPickerX.Services.Versions;
using ServerPickerX.Settings;
using ServerPickerX.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPickerX.Views
{
    public partial class MainWindow : Window
    {
        // Singleton instance for accessing the main window on execution lifetime
        public static MainWindow? Instance { get; private set; }

        public static bool IsDebugBuild
        {
            get
            {
                #if DEBUG
                    return true;
                #else
                    return false;
                #endif
            }
        }

        private ListSortDirection pingSortDirection = ListSortDirection.Ascending;
        private ListSortDirection packetLossSortDirection = ListSortDirection.Ascending;
        private bool _suppressPresetSelectionChanged;
        private PresetModel? _previousPreset;

        private readonly ILoggerService _loggerService;
        private readonly JsonSetting _jsonSetting;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IVersionService _versionService;
        private readonly ILocalizationService _localizationService;
        private readonly ServerDefinitionProvider _serverDefinitionProvider;

        // Parameterless constructor, allows design previewer to create its own instance since it doesn't support DI
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            _loggerService = ServiceLocator.GetRequiredService<ILoggerService>();
            _jsonSetting = ServiceLocator.GetRequiredService<JsonSetting>();
            _messageBoxService = ServiceLocator.GetRequiredService<IMessageBoxService>();
            _versionService = ServiceLocator.GetRequiredService<IVersionService>();
            _localizationService = ServiceLocator.GetRequiredService<ILocalizationService>();
            _serverDefinitionProvider = ServiceLocator.GetRequiredService<ServerDefinitionProvider>();
        }

        // DI constructor, allows inversion of control and unit tests mocking
        public MainWindow(
            ILoggerService loggerService,
            JsonSetting jsonSetting,
            IMessageBoxService messageBoxService,
            IVersionService versionService,
            ILocalizationService localizationService
            )
        {
            InitializeComponent();
            Instance = this;

            _loggerService = loggerService;
            _messageBoxService = messageBoxService;
            _versionService = versionService;
            _jsonSetting = jsonSetting;
            _localizationService = localizationService;
            _serverDefinitionProvider = ServiceLocator.GetRequiredService<ServerDefinitionProvider>();
        }

        private async void Window_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await InitializeApp();
        }

        private void GameModeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // Post to the UI thread with ContextIdle priority to ensure the ComboBox dropdown
            // is completely closed and all pointer captures are released before showing the modal.
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                await HandleGameModeChangeAsync();
            }, Avalonia.Threading.DispatcherPriority.ContextIdle);
        }



        private void TitleBar_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            e.Handled = true;
            var parentWindow = TopLevel.GetTopLevel(this) as Window;
            parentWindow?.BeginMoveDrag(e);
        }



        public async Task InitializeApp()
        {
            await _jsonSetting.LoadSettingsAsync();

            await SetLanguage();

            await ConfigureControls();

            var vm = ServiceLocator.GetRequiredService<MainWindowViewModel>();

            await vm.LoadServersAsync();

            DataContext = vm;

            if (vm.ServersLoaded)
            {
                await SyncServersAsync(vm);
                vm.LoadPresetPickerItems();
                await vm.RestoreLastSelectedPresetAsync();
            }

            ConfigurePresetControls(vm);
            RefreshClusterButtonContent();

            await _versionService.CheckVersionAsync();
        }

        private async Task SetLanguage()
        {
            // Extract language code from enum text
            var language = _jsonSetting.language.Replace(" ", "").Split("|")[1];

            await _localizationService.SetLanguage(language);
        }

        private async Task ConfigureControls()
        {
            try
            {
                IReadOnlyList<string> gameModes = _serverDefinitionProvider.GetGameModes();

                if (gameModes.Count == 0)
                {
                    throw new InvalidOperationException("No server definitions were found.");
                }

                if (!gameModes.Contains(_jsonSetting.game_mode, StringComparer.OrdinalIgnoreCase))
                {
                    await _jsonSetting.SetGameModeAsync(gameModes[0]);
                }

                GameModeComboBox.SelectionChanged -= GameModeComboBox_SelectionChanged;
                GameModeComboBox.ItemsSource = gameModes;
                GameModeComboBox.SelectedItem = _jsonSetting.game_mode;
                GameModeComboBox.SelectionChanged += GameModeComboBox_SelectionChanged;
            }
            catch (InvalidOperationException ex)
            {
                await _loggerService.LogErrorAsync("An error has occured while setting game mode combo box", ex.Message);

                throw;
            }

            RefreshClusterButtonContent();
        }

        private void ConfigurePresetControls(MainWindowViewModel vm)
        {
            SyncPresetSelection(vm.SelectedPreset);
        }

        private async Task SyncServersAsync(MainWindowViewModel vm)
        {
            var localRevision = await _jsonSetting.GetRevisionByGameModeAsync();

            var fetchedRevision = vm.GetServerDataService().GetFetchedRevision();

            string appId = _serverDefinitionProvider.GetAppIdByGameMode(_jsonSetting.game_mode);
            
            IReadOnlyList<string> affectedGameModes = _serverDefinitionProvider.GetGameModesByAppId(appId);
            
            bool hasAffectedPresets = affectedGameModes.Any(
                    gameMode => _jsonSetting.GetPresetsByGameMode(gameMode).Count > 0
                );

            // Store the initial revision without a reset when this game has no saved presets yet.
            if (localRevision == "-1" && !hasAffectedPresets)
            {
                await _jsonSetting.SetRevisionByGameModeAsync(fetchedRevision);
                return;
            }

            // Skip server unblocking and revision sync if local revision is equal to fetched revision
            if (localRevision == fetchedRevision)
            {
                return;
            }

            // This only happens on successful load and sync on startup or game switch
            await _messageBoxService.ShowMessageBoxAsync(
                    _localizationService.GetLocaleValue("MessageBoxInfoTitle"),
                    _localizationService.GetLocaleValue("SyncServersUnblockAllDialogue"),
                    MsBox.Avalonia.Enums.Icon.Setting
                    );

            // Unblock current game rules while preserving last selected preset
            bool unblocked = await vm.UnblockAllAsync(shouldClearLastSelectedPreset: false);

            if (!unblocked)
            {
                return;
            }

            await vm.PruneCurrentGamePresetEntriesAsync();

            if (affectedGameModes.Count > 1)
            {
                if (!await vm.PruneRelatedGamePresetEntriesAsync())
                {
                    return;
                }
            }

            await _jsonSetting.SetRevisionByGameModeAsync(fetchedRevision);
        }

        private async Task HandleGameModeChangeAsync()
        {
            if (DataContext is not MainWindowViewModel vm || GameModeComboBox?.SelectedItem == null)
            {
                return;
            }

            bool result = await _messageBoxService.ShowMessageBoxConfirmationAsync(
                    _localizationService.GetLocaleValue("MessageBoxInfoTitle"),
                    _localizationService.GetLocaleValue("SwapGameModeUnblockAllConflict"),
                    MsBox.Avalonia.Enums.Icon.Setting
                    );

            if (!result)
            {
                // Revert back selection without triggering event handler
                GameModeComboBox.SelectionChanged -= GameModeComboBox_SelectionChanged;
                GameModeComboBox.SelectedItem = _jsonSetting.game_mode;
                GameModeComboBox.SelectionChanged += GameModeComboBox_SelectionChanged;

                return;
            }

            // Clear the currently loaded game rules before changing game mode while preserving last selected preset
            await vm.UnblockAllAsync(shouldClearLastSelectedPreset: false);

            // Update json setting game mode and serialize it
            await _jsonSetting.SetGameModeAsync((string)GameModeComboBox.SelectedItem);

            await InitializeApp();
        }

        private async Task HandlePresetChangeAsync(
            PresetModel selectedPreset,
            PresetModel? previousPreset
            )
        {
            if (DataContext is not MainWindowViewModel vm)
            {
                return;
            }

            if (AreSamePresetSelection(selectedPreset, previousPreset))
            {
                return;
            }

            bool presetApplied = await vm.ApplyPresetAsync(selectedPreset);

            if (!presetApplied)
            {
                SyncPresetSelection(previousPreset);
                return;
            }

            SyncPresetSelection(vm.SelectedPreset);
            RefreshClusterButtonContent();
        }

        private void SyncPresetSelection(PresetModel? preset)
        {
            _suppressPresetSelectionChanged = true;
            // PresetComboBox removed
            _suppressPresetSelectionChanged = false;
            _previousPreset = preset;
        }

        private static bool AreSamePresetSelection(PresetModel? left, PresetModel? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Equals(right);
        }

        private void RefreshClusterButtonContent()
        {
            // Removed ClusterUnclusterBtn reference
        }
    }
}
