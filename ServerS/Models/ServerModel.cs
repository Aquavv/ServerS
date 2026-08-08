using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace ServerPickerX.Models
{
    // ObservableObject base class requires a partial class type to  
    // generate boiler plate code for common MVVM implementations
    public partial class ServerModel : ObservableObject
    {
        public string Flag { get; set; } = "";

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        [ObservableProperty]
        public string? ping;

        [ObservableProperty]
        public string? status;
         
        [ObservableProperty]
        public string? packetLoss;

        public List<RelayModel> RelayModels { get; set; } = [];

        [ObservableProperty]
        public string region = "NA"; // Default region

        [ObservableProperty]
        public bool isSelected = true;

        private CancellationTokenSource? _cancelTokenSource;

        public async void PingServer()
        {
            if (this._cancelTokenSource != null)
            {
                this._cancelTokenSource.Cancel();
            }

            this._cancelTokenSource = new CancellationTokenSource();
            var cancelToken = this._cancelTokenSource.Token;

            using var ping = new Ping();

            Ping = "Pinging server";

            RelayModel? bestRelay = null;
            long bestRtt = long.MaxValue;

            // Phase 1, Find the best relay (lowest RTT)
            foreach (RelayModel relay in RelayModels)
            {
                try
                {
                    string ipOnly = relay.IPv4.Contains("/") ? relay.IPv4.Split('/')[0] : relay.IPv4;
                    ipOnly = ipOnly.Contains(":") ? ipOnly.Split(':')[0] : ipOnly;
                    
                    if (ipOnly.EndsWith(".0")) 
                    {
                        ipOnly = ipOnly.Substring(0, ipOnly.Length - 2) + ".1";
                    }

                    var res = await ping.SendPingAsync(
                        address: IPAddress.Parse(ipOnly), 
                        timeout: TimeSpan.FromMilliseconds(800), 
                        options: new PingOptions(), 
                        cancellationToken: cancelToken
                        );

                    if (res.Status == IPStatus.Success && res.RoundtripTime >= 0 && res.RoundtripTime < bestRtt)
                    {
                        bestRtt = res.RoundtripTime;
                        bestRelay = relay;
                    }
                }
                catch (Exception) { }
            }

            // Cloud Endpoint Fallback for ICMP-blocking servers (e.g. Overwatch)
            if (bestRelay == null)
            {
                var cloudEndpoints = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "los angeles", "dynamodb.us-west-1.amazonaws.com" },
                    { "chicago", "dynamodb.us-east-2.amazonaws.com" },
                    { "são paulo", "dynamodb.sa-east-1.amazonaws.com" },
                    { "sao paulo", "dynamodb.sa-east-1.amazonaws.com" },
                    { "amsterdam", "dynamodb.eu-central-1.amazonaws.com" },
                    { "paris", "dynamodb.eu-west-3.amazonaws.com" },
                    { "seoul", "dynamodb.ap-northeast-2.amazonaws.com" },
                    { "tokyo", "dynamodb.ap-northeast-1.amazonaws.com" },
                    { "bahrain", "dynamodb.me-south-1.amazonaws.com" },
                    { "sydney", "dynamodb.ap-southeast-2.amazonaws.com" }
                };

                string fallbackTarget = "";
                foreach(var kvp in cloudEndpoints)
                {
                    if (Description.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        fallbackTarget = kvp.Value;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(fallbackTarget))
                {
                    try
                    {
                        var res = await ping.SendPingAsync(
                            hostNameOrAddress: fallbackTarget, 
                            timeout: TimeSpan.FromMilliseconds(2000), 
                            options: new PingOptions(), 
                            cancellationToken: cancelToken
                        );

                        if (res.Status == IPStatus.Success && res.RoundtripTime >= 0)
                        {
                            Ping = res.RoundtripTime + "ms";
                            Status = "Online";
                            PacketLoss = "0%";
                            return; // Fallback succeeded, we can exit early
                        }
                    }
                    catch (Exception) { }
                }
            }

            if (bestRelay != null)
            {
                PacketLoss = "Probing";

                // Phase 2, Probe the best relay 4 times
                int successCount = 0;
                long finalBestRtt = long.MaxValue;
                const int probeCount = 4;

                for (int i = 0; i < probeCount; i++)
                {
                    try
                    {
                        string ipOnly = bestRelay.IPv4.Contains("/") ? bestRelay.IPv4.Split('/')[0] : bestRelay.IPv4;
                        ipOnly = ipOnly.Contains(":") ? ipOnly.Split(':')[0] : ipOnly;

                        if (ipOnly.EndsWith(".0")) 
                        {
                            ipOnly = ipOnly.Substring(0, ipOnly.Length - 2) + ".1";
                        }

                        var res = await ping.SendPingAsync(
                            address: IPAddress.Parse(ipOnly), 
                            timeout: TimeSpan.FromMilliseconds(2000), 
                            options: new PingOptions(), 
                            cancellationToken: cancelToken
                            );

                        if (res.Status == IPStatus.Success && res.RoundtripTime >= 0)
                        {
                            successCount++;
                            finalBestRtt = Math.Min(finalBestRtt, res.RoundtripTime);
                        }
                    }
                    catch (Exception) { }
                }

                double lossPercent = (1 - (successCount / probeCount)) * 100;
                Ping = successCount > 0 ? finalBestRtt + "ms" : "";
                Status = successCount > 0 ? "✅" : "❌";
                PacketLoss = $"{lossPercent:F0}%";
            } else if (Ping == "Pinging server")
            {
                Ping = "";
                PacketLoss = "";
                Status = "❌";
            }
        }
    }
}
