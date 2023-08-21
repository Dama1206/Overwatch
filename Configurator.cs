﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Overwatch
{
    internal class Configurator
    {
        static string path = "settings.txt";
        static char commentChar = '!';

        public enum Datatypes
        {
            INT,
            DOUBLE,
            BOOL,
            STRING
        }
        public static void EnableStartOnBoot()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }
        public static string GetSettings()
        {
            string settings = "";

            string line;

            int amountOfSettings = 0;
            
            // This sets the amount of Settings for each not commented line which inherit '='
            using (StreamReader reader = new StreamReader(path)) 
            { 
                while((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        amountOfSettings++;
                    }
                }
            }

            // Read out the options' names
            string[] options = new string[amountOfSettings];
            char limiter = '=';
            int settingsIndex = 0;
            using(StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if(line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        string[] words = line.Split(' ');
                        line = "";
                        foreach (string word in words) { line += word; }
                        int index = line.IndexOf(limiter);
                        if (index > 0)
                        {
                            options[settingsIndex] = line.Substring(0, index);
                            settingsIndex++;
                        }
                    }
                    
                }
            }

            // Read out the values for the options
            string[] values = new string[options.Length];
            settingsIndex = 0;

            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        string[] words = line.Split(' ');
                        line = "";
                        foreach (string word in words) { line += word; }
                        int index = line.IndexOf(limiter);
                        if (index > 0)
                        {
                            values[settingsIndex] = line.Substring(index + 1);
                            settingsIndex++;
                        }

                    }
                }
            }

            settings += $"Total settings: {amountOfSettings}\n";
            for(int i = 0; i < settingsIndex; i++)
            {
                settings += $"{options[i]}:{values[i]} | {GetDatatype(values[i])}\n";
            }
            return settings;
        }

        public static string[,] Settings()
        {
            string[,] finalValues;

            int amountOfSettings = 0;

            string line;

            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        amountOfSettings++;
                    }
                }
            }

            // Read out the options' names
            string[] options = new string[amountOfSettings];
            char limiter = '=';
            int settingsIndex = 0;
            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        string[] words = line.Split(' ');
                        line = "";
                        foreach (string word in words) { line += word; }
                        int index = line.IndexOf(limiter);
                        if (index >= 0)
                        {
                            options[settingsIndex] = line.Substring(0, index);
                            settingsIndex++;
                        }
                    }

                }
            }

            // Read out the values for the options
            string[] values = OptionValues();

            // int amountOfSettings = AmountOfSettings();
            finalValues = new string[amountOfSettings, amountOfSettings];
            for(int i = 0; i < amountOfSettings; i++)
            {
                finalValues[0, i] = options[i];
            }
            for(int i = 0; i < amountOfSettings; i++)
            {
                finalValues[1, i] = values[i];
            }

            // Values are set like this:
            // for (int i = 0; i < amountOfSettings; i++)
            // {                        Value name            actual value
            //     Console.WriteLine($"{finalValues[0, i]}:{finalValues[1, i]}");
            // }
            return finalValues;
        }

        static int AmountOfSettings()
        {
            string line = "";
            int amountOfSettings = 0;
            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        amountOfSettings++;
                    }
                }
            }
            return amountOfSettings;
        }

        static string[] OptionNames()
        {
            int amountOfSettings = AmountOfSettings();
            string line = "";
            string[] options = new string[amountOfSettings];
            char limiter = '=';
            int settingsIndex = 0;
            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        string[] words = line.Split(' ');
                        line = "";
                        foreach (string word in words) { line += word; }
                        int index = line.IndexOf(limiter);
                        if (index >= 0)
                        {
                            options[settingsIndex] = line.Substring(0, index);
                            settingsIndex++;
                        }
                    }

                }
            }
            return options;
        }

        public static string[] OptionValues()
        {
            int amountOfSettings = AmountOfSettings();
            string line = "";
            string[] values = new string[OptionNames().Length];
            int settingsIndex = 0;
            char limiter = '=';

            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line.TrimStart()[0] != commentChar)
                    {
                        string[] words = line.Split(' ');
                        line = "";
                        foreach (string word in words) { line += word; }
                        int index = line.IndexOf(limiter);
                        if (index >= 0)
                        {
                            values[settingsIndex] = line.Substring(index + 1);
                            settingsIndex++;
                        }

                    }
                }
            }

            return values;
        }

        // Bool in settings.txt
        // public bool GetProperty(string varName)
        // {
        //     
        // }
        
        public static Datatypes GetDatatype(string input)
        {
            if (int.TryParse(input, out int n)) return Datatypes.INT;
            if (double.TryParse(input, out double d)) return Datatypes.DOUBLE;
            if (bool.TryParse(input, out bool b)) return Datatypes.BOOL;
            return Datatypes.STRING;

        }
    }
}
