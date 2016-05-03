﻿using StegomaticProject.StegoSystemUI.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using StegomaticProject.StegoSystemUI.Events;
using StegomaticProject.StegoSystemUI;

namespace StegomaticProject.StegoSystemUI
{
    public class StegoSystemWinForm : IStegoSystemUI
    {
        private Form1 _mainMenu { get; set; } // LAV ET INTERFACE HERTIL, DOG FØRST TIL SIDST.

        public StegoSystemWinForm()
        {
            //Creates new WinForms-window of Form1
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mainMenu = new Form1();
            Application.Run(_mainMenu);
        }

        public IConfig config { get; private set; }
        public string message { get; private set; }
        public string pathOfCoverImage { get; private set; }

        public event DisplayNotificationEventHandler NotifyUser;

        public void SetDisplayImage(Bitmap newImage)
        {
            
            throw new NotImplementedException();
        }

        public void ShowNotification(DisplayNotificationEvent e)
        {
            // Initialize a popup window and show the message!

            throw new NotImplementedException();
        }

        public void Start()
        {
            Console.ReadKey();
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}