﻿using System;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;
using Vesna.Business;
using Vesna.Forms;

namespace Vesna {
	internal static class Program {
		public static string User = "User";
		public static string PPVKName { get; set; }
		public static string ControlPlace { get; set; }
		public static RoadType CurrentRoadType { get; set; }	
		public static bool IsFederalRoad { get; set; }
		public static bool IsSoftRoad { get; set; }
		public static string ScaleNumber { get; set; }

		private static OleDbConnection _connection;
		private static readonly OleDbDataAdapter Adapter = new OleDbDataAdapter();
		private static readonly string MyDbPath = Application.StartupPath + @"\" + "database_inspector.mdb";
		private static string ConStr;

		[STAThread]
		private static void Main() {
			UpdateBaseFile(MyDbPath);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new InitForm());
		}

		public static void UpdateBaseFile(string path) {
			ConStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path +
																	 ";Jet OLEDB:Database Password=hfpdbnbt";
		}

		private static void ConOpen() {
			try {
				_connection = new OleDbConnection(ConStr);
				_connection.Open();
			} catch (Exception e) {
				MessageBox.Show(string.Format("Ошибка: не удалось загрузить базу данных\n{0}\n{1}", MyDbPath, e.Message));
			}
		}

		private static void ConClose() {
			try {
				_connection.Close();
			} catch (Exception e) {
				MessageBox.Show(string.Format("Ошибка: не удалось закрыть подключение к базе данных\n{0}\n{1}", MyDbPath, e.Message));
			}
		}

		public static int MakeAccess(string strcom) {
			var command = new OleDbCommand(strcom, _connection);
			return MakeAccess(command);
		}

		public static int MakeAccess(OleDbCommand com) {
			try {
				ConOpen();
				com.Connection = _connection;
				com.ExecuteScalar();
				ConClose();
				return 0;
			} catch (Exception e) {
				MessageBox.Show(string.Format("ОШИБКА: Не удалось выполнить запрос\n{0}\n{1}", com.CommandText, e.Message));
				return -1;
			}
		}

		public static DataTable GetAccess(string com) {
			if (com == string.Empty) {
				return null;
			}
			var dt = new DataTable();
			ConOpen();
			var command = new OleDbCommand(com, _connection);
			ConClose();

			try {
				Adapter.SelectCommand = command;
				Adapter.Fill(dt);
			} catch (Exception ex) {
				MessageBox.Show(string.Format("ОШИБКА: Не удалось выполнить запрос \n{0}\n{1}", com, ex.Message));
			}
			return dt;
		}

		/*public static void AddToLog(DateTime dateTime, string text) {
			MakeAccess(string.Format("INSERT INTO Log_base VALUES ( '{0}' , '{1}' , 'Пользователь' " + " )", dateTime.ToString("dd.MM.yyyy HH:mm:ss"), text));
		}*/
	}
}