//using System;

namespace SmartPermitService
{
    partial class SmartPermitService      {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        // protected override void OnStart(string[] args)
        //{
        //    System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "onStart.txt");
        //}
        //protected override void OnStop()
        //{
        //    System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "onStop.txt");
        //}
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "Service1";
        }

        #endregion
    }
}
