using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;
using Bluetooth.Abstractions.Scanning.Options;

using System;
using System.Collections.Generic;
using System.Text;

namespace MauiBleCerbotController.ViewModels
{
    public class ScannerViewModel
    {
        private readonly IBluetoothScanner _scanner;

        public IEnumerable<IBluetoothRemoteDevice>? Devices { get; }

        public ScannerViewModel(IBluetoothScanner scanner)
        {
            _scanner = scanner;

            // Subscribe to device discovery
            _scanner.DeviceListChanged += OnDeviceListChanged;
        }

        public async Task StartScanningAsync()
        {
            var options = new ScanningOptions
            {
                // Optional: Configure scanning behavior
            };

            await _scanner.StartScanningAsync(options);
        }

        private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
        {
            /*
            foreach (var device in _scanner.Devices)
            {
                Console.WriteLine($"Found: {device.Name} ({device.Id})");
                Console.WriteLine($"  RSSI: {device.SignalStrengthDbm} dBm");
            }
            */
        }
    }
}
