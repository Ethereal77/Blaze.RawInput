﻿// Copyright © 2020 Infinisis

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Blaze.Framework.RawInput.Sample
{
    /// <summary>
    ///   Program to show how to use RawInput.
    /// </summary>
    static class Program
    {
        /// <summary>
        ///   Main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var form = CreateWindow();

            // Information about the available devices
            ListDevices();

            // Setup the devices
            RegisterMouse(DeviceFlags.InputSink | DeviceFlags.DeviceNotify, form.Handle);
            RegisterKeyboard(DeviceFlags.None);

            // Ctrl + C closes the app
            Console.CancelKeyPress += (s, e) => Environment.Exit(0);
            Console.WriteLine();

            // Check for input every 500ms
            do
            {
                Thread.Sleep(500);

                RawInput.ProcessMessages();

                // This is only for the window to remain responsive and paint its contents
                Application.DoEvents();
            }
            while (true);
        }

        private static Form CreateWindow()
        {
            var form = new Form
            {
                Width = 300,
                Height = 200,

                Top = 10,
                Left = 10,

                Text = "Blaze RawInput sample",

                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Text = @"This is the window that will register itself to receive RawInput events.

Move the mouse, press kays on the keyboard or use a gamepad to view a log of the events generated by your actions."
            };
            form.Controls.Add(label);
            form.Visible = true;

            form.Show();

            return form;
        }

        private static void ListDevices()
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            foreach (var device in RawInput.Devices)
            {
                PrintDevice(device);
                Console.WriteLine();
            }

            Console.ForegroundColor = fg;
        }

        private static void PrintDevice(DeviceInfo device)
        {
            Console.WriteLine($"Device: 0x{device.Handle.ToString("X")} Type: {device.Type} Name: {device.Name}");
            Console.WriteLine(device switch
            {
                KeyboardInfo kbd => $"  Total keys: {kbd.TotalKeyCount}, Function Keys: {kbd.FunctionKeyCount}, Indicators: {kbd.IndicatorCount}, " +
                                    $"Type: {kbd.KeyboardType:x}-{kbd.Subtype:x}, Mode: {kbd.KeyboardMode:x}",

                MouseInfo mouse => $"  Id: {mouse.Id:x}, Buttons: {mouse.ButtonCount}, Sample rate: {mouse.SampleRate}, HWheel: {mouse.HasHorizontalWheel}",

                HidInfo hid => $"  Vendor: {hid.VendorId:x}, Product: {hid.ProductId:x}, Version: {hid.VersionNumber}, Usage page: {hid.UsagePage}, Usage: {hid.Usage}",

                _ => ""
            });
        }

        private static void RegisterMouse(DeviceFlags deviceFlags, IntPtr windowHandle = default)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"Registering mouse devices with flags [{deviceFlags}].");

            Console.ForegroundColor = fg;

            // We don't use message filtering as we'll be polling manually
            RawInput.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, deviceFlags, windowHandle);
            RawInput.MouseInput += OnMouseInput;
        }

        private static void RegisterKeyboard(DeviceFlags deviceFlags, IntPtr windowHandle = default)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"Registering keyboard devices with flags [{deviceFlags}].");

            Console.ForegroundColor = fg;

            // We don't use message filtering as we'll be polling manually
            RawInput.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, deviceFlags, windowHandle);
            RawInput.KeyboardInput += OnKeyboardInput;
        }

        private static void OnKeyboardInput(IntPtr device, IntPtr hwnd, in KeyboardInputEventArgs args)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"Window 0x{hwnd.ToString("X")}, Device 0x{device.ToString("X")}, Mode: {args.InputMode}: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Key: {args.Key}, Make code: {args.MakeCode}, State: {args.State}, ScanCodeFlags: {args.ScanCodeFlags}");
            Console.ForegroundColor = fg;
        }

        private static void OnMouseInput(IntPtr device, IntPtr hwnd, in MouseInputEventArgs args)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"Window 0x{hwnd.ToString("X")}, Device 0x{device.ToString("X")}, Mode: {args.InputMode}: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"(X:{args.X}, Y:{args.Y}), State: {args.Mode}, Buttons: {args.Buttons:x} {args.ButtonFlags}, Wheel: {args.WheelDelta}");
            Console.ForegroundColor = fg;
        }
    }
}
