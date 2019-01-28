﻿using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Vesna.Business.Data;
using Vesna.Controls;

namespace Vesna.Business {
	public class AutoRepository {
		public int Save(Auto auto) {
			if (auto.IsCanEdit) {
				string comAxis = string.Empty;
				string comAxisInBase = string.Empty;
				for (int i = 0; i < auto.AxisList.Count; i++) {
					comAxis += auto.AxisList[i].ToStringForBase() + ",";
					comAxisInBase += string.Format(" r_os_{0},  m_f_os_{0},  m_d_os_{0},  v_os_{0}, is_up_{0}, os_dmg_{0}, ", i + 1);
				}
				string com = "INSERT INTO MainTable ( "
				             + " Car_id, "
				             + " Data, "
				             //+ " VidTS, " -->tab_1_tip
				             + " RoadTypeId, "
				             + " punkt_name, "
				             + " address, "
				             + " vesi_date_OT, "
				             + " vesi_date_DO, "
				             + " vesi_zavod_nomer, "
				             //foto
				             //+ " harakter_narush, "

				             + " AutoTypeId, "
				             + " tab_1_mark, "
				             + " tab_1_model, "
				             + " tab_1_reg, "

				             + " tab_2_tip, "
				             + " tab_2_mark, "
				             + " tab_2_model, "
				             + " tab_2_reg, "

				             + " tab_3_tip, "
				             + " tab_3_mark, "
				             + " tab_3_model, "
				             + " tab_3_reg, "

				             + " sobstvenik, "
				             + " address_sobstvenik, "
				             + " organ, "
				             + " address_org, "
				             + " strana, "
				             + " sub, "
				             + " kog_strani, "


				             + " marshrut, "
				             + " marshrut_dlina, "
				             + " marshrut_kol_poezdok, "
				             + " harakter_gruza, "
				             + " vid_gruza, "

				             + " fakt_massa, "
				             + " dopus_massa, "
				             + " dmg_massa, "
				             + " count_os, "
				             + comAxisInBase

				             + " obyas_vodit, "
				             //+ " udostov_vodit, "
				             + " prin_mery, "

				             + " oper_ppvk, "
				             + " vodit, "
				             + " insp_police, "


				             + " primechanie, "

				             + " kom_index, "
				             + " razmer_usherba ) "
				             + " VALUES ( '"
				             //+ IsCanEdit + "' , '"
				             + auto.CarId + "' , '"
				             + auto.Datetime.ToString("dd.MM.yyyy HH:mm:ss") + "' , '"

				             + (int)auto.Road.RoadType + "' , '"
				             + auto.Ppvk + "' , '"
				             + auto.MestoKontrolya + "' , '"
				             + auto.Scales.CheckDateFrom + "' , '"
				             + auto.Scales.CheckDateTo + "' , '"
				             + auto.Scales.Number + "' , '"
				             //+ Foto + "' , '"
				             //+ HarakterNarush + "' , '"

				             + (int)auto.AutoType + "' , '"
				             + auto.Mark + "' , '"
				             + auto.Model + "' , '"
				             + auto.RegNumber + "' , "

				             + auto.Trailer1.ToStringForBase() + " , "
				             + auto.Trailer2.ToStringForBase() + " ,'"

				             + auto.Sobstvenik + "' , '"
				             + auto.SobstvenikAddess + "' , '"
				             + auto.OrganName + "' , '"
				             + auto.OrganAddress + "' , '"
				             + auto.Road.County + "' , '"
				             + auto.Road.Region + "' , '"
				             + auto.Road.RegionKode + "' , '"

				             + auto.Road.WayText + "' , '"
				             + auto.Road.Distance + "' , '"
				             + auto.Road.CountWays + "' , '"
				             + auto.HarakterGruza + "' , '"
				             + auto.VidGruza + "' , '"

				             + auto.FullWeightData.Value + "' , '"
				             + auto.FullWeightData.Limit + "' , '"
				             + auto.FullWeightData.Damage + "' , '"
				             + auto.AxisList.Count + "' ,  "
				             + comAxis + " '"

				             + auto.VoditelObyasnenie + "' , '"
				             //+ auto.Voditel_udostoverenie + "' , '"
				             + auto.PrinyatieMery + "' , '"

				             + auto.ImyaOperator + "' , '"
				             + auto.ImyaVodit + "' , '"
				             + auto.ImyaInspektora + "' , '"

				             + auto.Primechanie + "' , '"

				             + auto.SpecIndex + "' , '"
				             + auto.FullAutoDamage + "'"
				             + " )";

				var command = new OleDbCommand(com);
				auto.IsCanEdit = false;

				if (Program.MakeAccess(command) >= 0) {
					auto.Id = int.Parse(Program.GetAccess("SELECT TOP 1 id FROM MainTable ORDER BY id DESC ").Rows[0]["id"].ToString());
					try {
						if (auto.Foto != null) {
							string fullPath = $"{Application.StartupPath}\\Foto\\{auto.Id}.jpg";
							if (!File.Exists(fullPath)) {
								auto.Foto.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
							}
						}
					} catch (Exception e) {
						MessageBox.Show($"Ошибка сохранения изображения\n{e.Message}");
						return 1;
					}
					return 0;
				}
				return -1;
			}
			return 0;
		}

		public Auto LoadAuto(int id) {
			var auto = new Auto();
			auto.Id = id;
			auto.IsCanEdit = false;

			#region FillFromBase

			DataTable dt = Program.GetAccess("SELECT * FROM MainTable WHERE id = " + id);

			auto.Id = id;
			auto.CarId = int.Parse(dt.Rows[0]["car_id"].ToString());
			auto.Datetime = TryParseDTOrGetDefault(dt.Rows[0]["Data"].ToString(), "Data");
			//+ " VidTS, " -->tab_1_tip
			auto.Road.RoadType = (RoadType)(int.Parse(dt.Rows[0]["RoadTypeId"].ToString()));
			auto.Ppvk = dt.Rows[0]["punkt_name"].ToString();
			auto.MestoKontrolya = dt.Rows[0]["address"].ToString();
			auto.Scales.CheckDateFrom = TryParseDTOrGetDefault(dt.Rows[0]["vesi_date_OT"].ToString(), "vesi_date_OT");
			auto.Scales.CheckDateTo = TryParseDTOrGetDefault(dt.Rows[0]["vesi_date_DO"].ToString(), "vesi_date_DO");
			auto.Scales.Number = dt.Rows[0]["vesi_zavod_nomer"].ToString();

			string strpath = Application.StartupPath + @"\Foto\" + auto.Id + ".jpg";
			if (File.Exists(strpath)) {
				auto.Foto = Image.FromFile(strpath);
			}
			auto.AutoType = (AutoType)(int.Parse(dt.Rows[0]["AutoTypeId"].ToString()));
			auto.Mark = dt.Rows[0]["tab_1_mark"].ToString();
			auto.Model = dt.Rows[0]["tab_1_model"].ToString();
			auto.RegNumber = dt.Rows[0]["tab_1_reg"].ToString();

			auto.Trailer1.Kind = dt.Rows[0]["tab_2_tip"].ToString();
			auto.Trailer1.Mark = dt.Rows[0]["tab_2_mark"].ToString();
			auto.Trailer1.Model = dt.Rows[0]["tab_2_model"].ToString();
			auto.Trailer1.RegNomer = dt.Rows[0]["tab_2_reg"].ToString();
			auto.Trailer2.Kind = dt.Rows[0]["tab_3_tip"].ToString();
			auto.Trailer2.Mark = dt.Rows[0]["tab_3_mark"].ToString();
			auto.Trailer2.Model = dt.Rows[0]["tab_3_model"].ToString();
			auto.Trailer2.RegNomer = dt.Rows[0]["tab_3_reg"].ToString();

			auto.Sobstvenik = dt.Rows[0]["sobstvenik"].ToString();
			auto.SobstvenikAddess = dt.Rows[0]["address_sobstvenik"].ToString();
			auto.OrganName = dt.Rows[0]["organ"].ToString();
			auto.OrganAddress = dt.Rows[0]["address_org"].ToString();
			auto.Road.County = dt.Rows[0]["strana"].ToString();
			auto.Road.Region = dt.Rows[0]["sub"].ToString();

			auto.Road.WayText = dt.Rows[0]["marshrut"].ToString();
			auto.Road.RegionKode = TryParseIntOrGetDefault(dt.Rows[0]["kog_strani"].ToString(), "kog_strani");
			auto.Road.Distance = TryParseFloatOrGetDefault(dt.Rows[0]["marshrut_dlina"].ToString(), "marshrut_dlina");
			auto.Road.CountWays = TryParseIntOrGetDefault(dt.Rows[0]["marshrut_kol_poezdok"].ToString(), "marshrut_kol_poezdok");

			auto.HarakterGruza = dt.Rows[0]["harakter_gruza"].ToString();
			auto.VidGruza = dt.Rows[0]["vid_gruza"].ToString();

			auto.FullWeightData.Value = TryParseFloatOrGetDefault(dt.Rows[0]["fakt_massa"].ToString(), "fakt_massa");
			auto.FullWeightData.Limit = TryParseFloatOrGetDefault(dt.Rows[0]["dopus_massa"].ToString(), "dopus_massa");
			auto.FullWeightData.Damage = TryParseFloatOrGetDefault(dt.Rows[0]["dmg_massa"].ToString());
			int count = TryParseIntOrGetDefault(dt.Rows[0]["count_os"].ToString(), "count_os");
			for (int i = 1; i <= count; i++) {
				int v = int.Parse(dt.Rows[0]["v_os_" + i].ToString());
				float nf = TryParseFloatOrGetDefault(dt.Rows[0]["m_f_os_" + i].ToString());
				float nd = TryParseFloatOrGetDefault(dt.Rows[0]["m_d_os_" + i].ToString());
				float rs = TryParseFloatOrGetDefault(dt.Rows[0]["r_os_" + i].ToString());
				float dmg = TryParseFloatOrGetDefault(dt.Rows[0]["os_dmg_" + i].ToString());
				bool isUp = TryParseBoolOrGetDefault(dt.Rows[0]["is_up_" + i].ToString());
				auto.AddAxis((AxisType)v, isUp, rs, nf, nd, dmg);
			}
			auto.VoditelObyasnenie = dt.Rows[0]["obyas_vodit"].ToString();
			auto.PrinyatieMery = dt.Rows[0]["prin_mery"].ToString();

			auto.ImyaOperator = dt.Rows[0]["oper_ppvk"].ToString();
			auto.ImyaVodit = dt.Rows[0]["vodit"].ToString();
			auto.ImyaInspektora = dt.Rows[0]["insp_police"].ToString();

			auto.Primechanie = dt.Rows[0]["primechanie"].ToString();

			auto.SpecIndex = TryParseFloatOrGetDefault(dt.Rows[0]["kom_index"].ToString(), "kom_index");
			auto.FullAutoDamage = TryParseFloatOrGetDefault(dt.Rows[0]["razmer_usherba"].ToString(), "razmer_usherba");

			#endregion
			
			/*Calculator.GetMassDamage();
			for (int i = 0; i < AxisList.Count; i++) {
				Calculator.GetAxisDamage(i);
			}*/
			return auto;
		}

		private static float TryParseFloatOrGetDefault(string value, string fieldName = "", float def = 0) {
			if (float.TryParse(value, out float pValue)) {
				return pValue;
			}
			if (!string.IsNullOrEmpty(fieldName)) {
				MessageBox.Show("Parse Error: " + fieldName);
			}
			return def;
		}

		private static int TryParseIntOrGetDefault(string value, string fieldName, int def = 0) {
			if (int.TryParse(value, out int pValue)) {
				return pValue;
			}
			if (!string.IsNullOrEmpty(fieldName)) {
				MessageBox.Show("Parse Error: " + fieldName);
			}
			return def;
		}

		private static DateTime TryParseDTOrGetDefault(string value, string fieldName, DateTime def = new DateTime()) {
			if (DateTime.TryParse(value, out DateTime pValue)) {
				return pValue;
			}
			if (!string.IsNullOrEmpty(fieldName)) {
				MessageBox.Show("Parse Error: " + fieldName);
			}
			return def;
		}

		private static bool TryParseBoolOrGetDefault(string value, string fieldName = "", bool def = false) {
			if (bool.TryParse(value, out bool pValue)) {
				return pValue;
			}
			if (!string.IsNullOrEmpty(fieldName)) {
				MessageBox.Show("Parse Error: " + fieldName);
			}
			return def;
		}
	}
}
