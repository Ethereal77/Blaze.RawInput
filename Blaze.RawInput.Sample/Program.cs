﻿// Copyright © 2020 Infinisis

using System;
using System.Drawing;
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

            // Listen to device changes
            RawInput.DeviceChanged += OnDeviceChanged;

            Console.WriteLine();
            Application.Run(form);
        }

        private static Form CreateWindow()
        {
            var form = new Form
            {
                Width = 300,
                Height = 200,

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

            return form;
        }

        private static void ListDevices()
        {
            foreach (var device in RawInput.Devices)
            {
                Console.WriteLine($"Device: {device.Handle} Type: {device.Type} Name: {device.Name}");
                Console.WriteLine(device switch
                {
                    KeyboardInfo kbd => $"  Total keys: {kbd.TotalKeyCount}, Function Keys: {kbd.FunctionKeyCount}, Indicators: {kbd.IndicatorCount}, " +
                                        $"Type: {kbd.KeyboardType}-{kbd.Subtype}, Mode: {kbd.KeyboardMode}",

                    MouseInfo mouse => $"  Id: {mouse.Id}, Buttons: {mouse.ButtonCount}, Sample rate: {mouse.SampleRate}, HWheel: {mouse.HasHorizontalWheel}",

                    HidInfo hid => $"  Vendor: {hid.VendorId}, Product: {hid.ProductId}, Version: {hid.VersionNumber}, Usage page: {hid.UsagePage}, Usage: {hid.Usage}",

                    _ => ""
                });
                Console.WriteLine();
            }
        }

        private static void RegisterMouse(DeviceFlags deviceFlags, IntPtr windowHandle = default)
        {
            Console.WriteLine($"Registering mouse devices with flags [{deviceFlags}].");

            RawInput.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, deviceFlags, windowHandle);
            RawInput.MouseInput += OnMouseInput;
        }

        private static void RegisterKeyboard(DeviceFlags deviceFlags, IntPtr windowHandle = default)
        {
            Console.WriteLine($"Registering keyboard devices with flags [{deviceFlags}].");

            RawInput.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, deviceFlags, windowHandle);
            RawInput.KeyboardInput += OnKeyboardInput;
        }

        private static void OnKeyboardInput(IntPtr device, IntPtr hwnd, in KeyboardInputEventArgs args)
        {
            var key = Enum.GetName(typeof(Keys), args.Key);
            var state = Enum.GetName(typeof(KeyState), args.State);
            var flags = Enum.GetName(typeof(ScanCodeFlags), args.ScanCodeFlags);

            Console.Write($"Window {hwnd:x}, Device {device:x}: ");
            Console.WriteLine($"Key: {key}, State: {state}, ScanCodeFlags: {flags}, Make code: {args.MakeCode}");
        }

        private static void OnMouseInput(IntPtr device, IntPtr hwnd, in MouseInputEventArgs args)
        {
            var buttonFlags = Enum.GetName(typeof(MouseButtonFlags), args.ButtonFlags);
            var mode = Enum.GetName(typeof(MouseMode), args.Mode);

            Console.Write($"Window {hwnd:x}, Device {device:x}: ");
            Console.WriteLine($"(x,y):({args.X},{args.Y}), Buttons: {args.Buttons} {buttonFlags}, State: {mode}, Wheel: {args.WheelDelta}");
        }

        private static void OnDeviceChanged(IntPtr device, DeviceChange change)
        {
            Console.Write($"Device {device:x}: ");
            Console.WriteLine(change switch
            {
                DeviceChange.Arrival => "The device has been added to the system.",
                DeviceChange.Removal => "The device has been removed from the system.",

                _ => "Unknown device change notification!"
            });
        }
    }
}
