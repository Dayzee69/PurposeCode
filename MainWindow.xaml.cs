using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;

namespace PurposeCode
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            datePicker.SelectedDate = DateTime.Now;
        }

        private void GetButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (docNumTb.Text == "") 
                {
                    throw new Exception("Номер документа не может быть пустым");
                }
                getPurposeCode();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void getPurposeCode()
        {

            nameTb.Text = "";
            purpuseCodeTb.Text = "";
            valueTb.Text = "";
            DateTime date1 = (DateTime)datePicker.SelectedDate;
            DateTime date2 = date1.AddHours(23).AddMinutes(59).AddSeconds(59);
            DirectoryInfo directoryInfo = new DirectoryInfo(@"\\путь");
            List<FileInfo> dirs = directoryInfo.GetFiles("requ*.xml", SearchOption.TopDirectoryOnly).Where(t => t.CreationTime >= date1 
            && t.CreationTime <= date2).ToList();

            foreach (FileInfo doc in dirs)
            {

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(doc.FullName);

                XmlElement xPayment = xDoc.DocumentElement;
                XmlNode xDocNum = xPayment.Attributes.GetNamedItem("doc-number");
                if (xDocNum.InnerText == docNumTb.Text) 
                {
                    XmlNode xPurposeCode = xPayment.Attributes.GetNamedItem("purpose-code");
                    XmlNodeList xPayerList = xDoc.GetElementsByTagName("payer");
                    XmlNodeList xAmountList = xDoc.GetElementsByTagName("amount");
                    XmlNode xName = xPayerList[0].Attributes.GetNamedItem("name");
                    XmlNode xValue = xAmountList[0].Attributes.GetNamedItem("value");
                    valueTb.Text = xValue.InnerText;
                    nameTb.Text = xName.InnerText;
                    if (xPurposeCode != null)
                    {
                        purpuseCodeTb.Text = xPurposeCode.InnerText;
                    }
                    else
                    {
                        purpuseCodeTb.Text = "Не указан";
                    }
                    break;
                }

            }

            if (valueTb.Text == "" && nameTb.Text == "" && purpuseCodeTb.Text == "") 
            {
                throw new Exception("Документ не найден");
            }
        }
    }
}
