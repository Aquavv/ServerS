using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using ServerPickerX.Helpers;
using ServerPickerX.Services.Loggers;
using ServerPickerX.Services.Processes;
using ServerPickerX.Views;
using System;
using System.Threading.Tasks;

namespace ServerPickerX.Services.MessageBoxes
{
    public class MessageBoxService : IMessageBoxService
    {
        private readonly IProcessService _processService;
        private readonly ILoggerService _logger;

        public MessageBoxService(ILoggerService logger, IProcessService processService)
        {
            _logger = logger;
            _processService = processService;
        }

        public async Task ShowMessageBoxAsync(string title, string text, Icon icon = Icon.Info)
        {
            try
            {
                var dialog = new CustomDialogWindow
                {
                    TitleText = title,
                    Message = text,
                    ShowCancel = false,
                    IconValue = icon == Icon.Error ? "fa-circle-xmark" : "fa-circle-info",
                    IconColor = icon == Icon.Error ? "#ef4444" : "#00f2ff"
                };

                await dialog.ShowDialog<string>(MainWindow.Instance!);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Failed to show message box", ex.Message);
            }
        }

        public async Task<bool> ShowMessageBoxConfirmationAsync(string title, string text, Icon icon = Icon.Info)
        {
            try
            {
                var dialog = new CustomDialogWindow
                {
                    TitleText = title,
                    Message = text,
                    ShowCancel = true,
                    IconValue = icon == Icon.Error ? "fa-circle-xmark" : "fa-circle-info",
                    IconColor = icon == Icon.Error ? "#ef4444" : "#00f2ff"
                };

                var result = await dialog.ShowDialog<string>(MainWindow.Instance!);
                return result == "Ok";
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Failed to show message box confirmation", ex.Message);
                return false;
            }
        }

        public async Task ShowMessageBoxWithLinkAsync(string title, string text, string url, Icon icon = Icon.Info)
        {
            try
            {
                var dialog = new CustomDialogWindow
                {
                    TitleText = title,
                    Message = text,
                    ShowCancel = true,
                    IconValue = icon == Icon.Error ? "fa-circle-xmark" : "fa-circle-info",
                    IconColor = icon == Icon.Error ? "#ef4444" : "#00f2ff"
                };

                var result = await dialog.ShowDialog<string>(MainWindow.Instance!);

                if (result == "Ok")
                {
                    await _processService.OpenUrl(url);
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Failed to show message box with hyperlink", ex.Message);
            }
        }
    }
}