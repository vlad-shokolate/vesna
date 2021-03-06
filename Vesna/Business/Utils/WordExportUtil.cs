using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Vesna.Business.Data;
using Word = Microsoft.Office.Interop.Word;

namespace Vesna.Business.Utils {
	internal static class WordExportUtil {
		private static Word.Application _wordApp;

		public static void Export(Auto curTc) {
			Fill(curTc);
			_wordApp.Visible = true;
		}

		public static void Print(Auto curTc) {
			Fill(curTc);
			_wordApp.PrintOut();
		}

		private static void Fill(Auto auto) {
			try {
				string fileName = $"{Application.StartupPath}\\Files\\act.doc";
				_wordApp = new Word.Application();

				Word.Document doc = _wordApp.Documents.Add(fileName);

				Replace("[ДОК_НОМЕР]", auto.CarId + "." + auto.Id, ref doc); ///////////////////////////////////////
				Replace("[ДАТА]", auto.Datetime.ToString("dd.MM.yyyy HH:mm:ss"), ref doc);
				Replace("[НОМЕР_ПВК]", auto.Ppvk, ref doc);
				Replace("[МЕСТО_КОНТР]", auto.MestoKontrolya, ref doc);
				Replace("[ВИД_ДОРОГИ]", SpravochnikUtil.GetRoadDescription(auto.Road.RoadType), ref doc);
				Replace("[ДАТА_ПОВЕРКИ]", auto.Scales.CheckDateFrom.ToString("dd.MM.yyyy"), ref doc);
				Replace("[ДАТА_ОКОНЧАНИЯ_ПОВЕРКИ]", auto.Scales.CheckDateTo.ToString("dd.MM.yyyy"), ref doc);
				Replace("[ЗАВОД_НОМЕР_ВЕСОВ]", auto.Scales.Number, ref doc);
				Replace("[ХАРАКТЕР_НАРУШЕНИЯ]", GetHarakterNarush(auto), ref doc);

				Replace("[ТАБ_ТИП_1]", SpravochnikUtil.GetAutoName(auto.AutoType), ref doc);
				Replace("[ТАБ_ТИП_2]", auto.Trailer1.Kind, ref doc);
				Replace("[ТАБ_ТИП_3]", auto.Trailer2.Kind, ref doc);
				Replace("[ТАБ_МАРКА_1]", auto.Mark, ref doc);
				Replace("[ТАБ_МАРКА_2]", auto.Trailer1.Mark, ref doc);
				Replace("[ТАБ_МАРКА_3]", auto.Trailer2.Mark, ref doc);
				Replace("[ТАБ_МОДЕЛЬ_1]", auto.Model, ref doc);
				Replace("[ТАБ_МОДЕЛЬ_2]", auto.Trailer1.Model, ref doc);
				Replace("[ТАБ_МОДЕЛЬ_3]", auto.Trailer2.Model, ref doc);
				Replace("[ТАБ_РЕГ_НОМ_1]", auto.RegNumber, ref doc);
				Replace("[ТАБ_РЕГ_НОМ_2]", auto.Trailer1.RegNomer, ref doc);
				Replace("[ТАБ_РЕГ_НОМ_3]", auto.Trailer2.RegNomer, ref doc);

				Replace("[ИМЯ_СОБСТВЕНИКА]", auto.Sobstvenik, ref doc);
				Replace("[АДРЕС_СОБСТННИКА]", auto.SobstvenikAddess, ref doc);
				Replace("[№_СВИД-ВА]", auto.SobstvenikSvidetelstvo, ref doc);
				Replace("[ИМЯ_СОБСТВЕННИКА_ПРИЦЕПА]", auto.SobstvenikPricep, ref doc);
				Replace("[АДРЕС_СОБСТННИКА_ПРИЦЕПА]", auto.SobstvenikPricepAddess, ref doc);
				Replace("[№_СВИД-ВА_ПРИЦЕПА]", auto.SobstvenikPricepSvidetelstvo, ref doc);
				Replace("[ФИО_ВОД]", auto.Driver, ref doc);
				Replace("[СЕРИЯ_И_№_ВОД_УДОСТ-Я]", auto.DriverLicense, ref doc);
				//Replace("[ИМЯ_ОРГАНИЗАЦИИ]", auto.OrganName, ref doc);
				//Replace("[АДР_ОРГ]", auto.OrganAddress, ref doc);

				Replace("[МЕСТО_КОНТР]", auto.MestoKontrolya, ref doc);

				Replace("[СТРАНА_СОБ]", auto.Road.County, ref doc);
				Replace("[СУБЪЕКТ_СОБ]", auto.Road.Region, ref doc);
				Replace("[КОД_СУБ]", auto.Road.RegionKode.ToString(), ref doc);

				Replace("[МАРШРУТ]", auto.Road.WayText, ref doc);
				Replace("[КМ_ОБЩЕЕ]", auto.Road.Distance.ToString(), ref doc);
				Replace("[ХАРАК_ПЕРЕВОЗ]", auto.HarakterGruza, ref doc);

				Replace("[ВИД_ГРУЗ]", auto.VidGruza, ref doc);
				Replace("[МАССА_ДОПУСТ]", auto.FullWeightData.Limit.ToString(), ref doc);
				Replace("[МАССА_ФАКТИЧ]", auto.FullWeightData.Value.ToString(), ref doc);
				Replace("[МАССА_ПРЕВЫШЕНИЕ]", auto.FullWeightData.Over.ToString(), ref doc);
				Replace("[МАССА_ПРЕВЫШЕНИЕ_ПРОЦ]", auto.FullWeightData.PercentageExceeded + "%", ref doc);

				string tempStringRazmerVredaOs = string.Empty;
				float tempSumRazmerVreda = 0;
				for (int i = 0; i < 10; i++) {
					string axisDistanceToNext = string.Empty;
					string axisDistanceToNextWithInaccuracy = string.Empty;
					string axisWeightValue = string.Empty;
					string axisWeightValueWithInaccuracy = string.Empty;
					string axisLoadLimit = string.Empty;
					string axisOver = string.Empty;
					string axisOverPercent = string.Empty;
					bool isPnevmo = false;
					bool isDouble = false;

					if (i < auto.AxisList.Count) {
						Axis axis = auto.AxisList[i];
						axisWeightValue = axis.WeightValue.ToString(CultureInfo.InvariantCulture);
						axisWeightValueWithInaccuracy = axis.WeightValueWithInaccuracy.ToString(CultureInfo.InvariantCulture);
						axisLoadLimit = axis.LoadLimit <= 0 ? "-" : axis.LoadLimit.ToString(CultureInfo.InvariantCulture);
						axisOver = Math.Round(axis.GetOver(), 2).ToString(CultureInfo.InvariantCulture);
						axisOverPercent = Math.Round(axis.GetOverPercent(), 2) + "%";
						isPnevmo = axis.IsPnevmo;
						isDouble = axis.IsDouble;
						tempStringRazmerVredaOs += axis.Damage + ((auto.AxisList.Count - 1 != i) ? "+" : "=");
						tempSumRazmerVreda += axis.Damage;
						if (i < auto.AxisList.Count - 1) {
							axisDistanceToNext = axis.DistanceToNext.ToString(CultureInfo.InvariantCulture);
							axisDistanceToNextWithInaccuracy = axis.DistanceToNextWithInaccuracy.ToString(CultureInfo.InvariantCulture);
						}
					}

					Replace("[О" + (i + 1) + "]", axisDistanceToNext, ref doc);
					Replace("[ОИ" + (i + 1) + "]", axisDistanceToNextWithInaccuracy, ref doc);
					Replace("[Н_Ф" + (i + 1) + "]", axisWeightValue, ref doc);
					Replace("[Н_И" + (i + 1) + "]", axisWeightValueWithInaccuracy, ref doc);
					Replace("[Н_Д" + (i + 1) + "]", axisLoadLimit, ref doc);
					Replace("[Н_П" + (i + 1) + "]", axisOver, ref doc);
					Replace("[Н_Р" + (i + 1) + "]", axisOverPercent, ref doc);
					Replace("[Д" + (i + 1) + "]", $"{(isDouble ? "+" : "-")} / {(isPnevmo ? "+" : "-")}", ref doc);
				}

				string[] groupInfos = auto.AxisList.Select(a => a.BlockInfo).Distinct().ToArray();
				for (int i = 0; i < 10; i++) {
					if (i >= groupInfos.Length) {
						Replace($"[ИНФО_О_ГРУППЕ{i}]", string.Empty, ref doc);
					} else {
						Replace($"[ИНФО_О_ГРУППЕ{i}]", groupInfos[i], ref doc);
					}
				}
				tempStringRazmerVredaOs += tempSumRazmerVreda;
				Replace("[ОБЬЯС_ВОД]", auto.VoditelObyasnenie, ref doc);
				Replace("[ФИО_ОПР]", auto.OperatorPvk, ref doc);
				Replace("[ФИО_ИНСП]", auto.InspectorGibdd, ref doc);
				Replace("[ФИО_ВОД]", auto.Driver, ref doc);
				Replace("[ФИО_ВОД]", auto.Driver, ref doc);
				Replace("[РАЗМ_УЩЕРБА]", auto.FullAutoDamage.ToString(), ref doc);

				Replace("[РАЗМЕР_ВРЕДА_МАССА]", auto.FullWeightData.Damage.ToString(), ref doc);
				Replace("[РАЗМЕР_ВРЕДА_ОСИ_STRING]", tempStringRazmerVredaOs, ref doc);
				Replace("[КОМ_ИНДЕКС]", auto.SpecIndex.ToString(), ref doc);

				if (auto.Foto != null) {
					try {
						Word.Range rr = doc.Range();
						rr.Find.Execute(FindText: "[ИЗОБРАЖЕНИЕ]", ReplaceWith: "");
						doc.Shapes.AddPicture($@"{Application.StartupPath}\Files\Foto\{auto.Id}.jpg", Width: 125, Height: 125, Anchor: rr);
					} catch (Exception e) {
						MessageBox.Show("Не удалось добавить в акт изображение\n" + e.Message);
					}
				} else {
					Replace("[ИЗОБРАЖЕНИЕ]", string.Empty, ref doc);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private static void Replace(string search, string replace, ref Word.Document doc) {
			Word.Range r = doc.Range();
			r.Find.Execute(FindText: search, ReplaceWith: replace);
		}

		public static string GetHarakterNarush(Auto auto) {
			bool weightOver = auto.FullWeightData.HasOver;
			bool axisOver = auto.HasAxisOver;

			if (weightOver && axisOver) {
				return "с превышением предельно допустимой массы транспортных средств и с превышением предельно допустимых осевых нагрузок";
			}
			if (weightOver) {
				return "с превышением предельно допустимой массы транспортных средств";
			}
			if (axisOver) {
				return "с превышением предельно допустимых осевых нагрузок";
			}
			return "без превышения допустимой массы и допустимых осевых нагрузок";
		}
	}
}