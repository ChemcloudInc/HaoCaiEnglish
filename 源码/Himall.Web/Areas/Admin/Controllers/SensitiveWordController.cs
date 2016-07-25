using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class SensitiveWordController : BaseAdminController
	{
		public SensitiveWordController()
		{
		}

		public JsonResult Add(string word, string category)
		{
			ISensitiveWordService sensitiveWordService = ServiceHelper.Create<ISensitiveWordService>();
			if (sensitiveWordService.ExistSensitiveWord(word))
			{
				Result result = new Result()
				{
					success = false,
					msg = "该敏感关键词已存在！"
				};
				return Json(result);
			}
			SensitiveWordsInfo sensitiveWordsInfo = new SensitiveWordsInfo()
			{
				SensitiveWord = word,
				CategoryName = category
			};
			sensitiveWordService.AddSensitiveWord(sensitiveWordsInfo);
			Result result1 = new Result()
			{
				success = true,
				msg = "添加成功！"
			};
			return Json(result1);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult BatchDelete(string ids)
		{
			string[] strArrays = ids.Split(new char[] { ',' });
			List<int> nums = new List<int>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				nums.Add(Convert.ToInt32(strArrays1[i]));
			}
			ServiceHelper.Create<ISensitiveWordService>().BatchDeleteSensitiveWord(nums.ToArray());
			Result result = new Result()
			{
				success = true,
				msg = "批量删除成功！"
			};
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(int id)
		{
			ServiceHelper.Create<ISensitiveWordService>().DeleteSensitiveWord(id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		public JsonResult Eidt(int id, string word, string category)
		{
			SensitiveWordsInfo sensitiveWordsInfo = new SensitiveWordsInfo()
			{
				Id = id,
				SensitiveWord = word,
				CategoryName = category
			};
			ServiceHelper.Create<ISensitiveWordService>().UpdateSensitiveWord(sensitiveWordsInfo);
			Result result = new Result()
			{
				success = true,
				msg = "修改成功！"
			};
			return Json(result);
		}

		public JsonResult GetCategories()
		{
			return Json(ServiceHelper.Create<ISensitiveWordService>().GetCategories());
		}

		private static string GetCellValue(Cell cell)
		{
			string str;
			if (cell == null)
			{
				return string.Empty;
			}
			switch (cell.CellType)
			{
				case CellType.Unknown:
				case CellType.NUMERIC:
				{
					return cell.ToString();
				}
				case CellType.STRING:
				{
					return cell.StringCellValue;
				}
				case CellType.FORMULA:
				{
					try
					{
						HSSFFormulaEvaluator hSSFFormulaEvaluator = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
						hSSFFormulaEvaluator.EvaluateInCell(cell);
						str = cell.ToString();
					}
					catch
					{
						str = cell.NumericCellValue.ToString();
					}
					return str;
				}
				case CellType.BLANK:
				{
					return string.Empty;
				}
				case CellType.BOOLEAN:
				{
					return cell.BooleanCellValue.ToString();
				}
				case CellType.ERROR:
				{
					return cell.ErrorCellValue.ToString();
				}
				default:
				{
					return cell.ToString();
				}
			}
		}

		public DataTable GetData(string filePath)
		{
			HSSFWorkbook hSSFWorkbook;
			DataTable dataTable;
			try
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					hSSFWorkbook = new HSSFWorkbook(fileStream);
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
			using (Sheet sheetAt = hSSFWorkbook.GetSheetAt(0))
			{
				DataTable dataTable1 = new DataTable();
				Row row = sheetAt.GetRow(0);
				int lastCellNum = row.LastCellNum;
				int lastRowNum = sheetAt.LastRowNum;
				for (int i = row.FirstCellNum; i < lastCellNum; i++)
				{
					DataColumn dataColumn = new DataColumn(row.GetCell(i).StringCellValue);
					dataTable1.Columns.Add(dataColumn);
				}
				for (int j = sheetAt.FirstRowNum + 1; j <= lastRowNum; j++)
				{
					Row row1 = sheetAt.GetRow(j);
					DataRow cellValue = dataTable1.NewRow();
					if (row1 != null)
					{
						for (int k = row1.FirstCellNum; k < lastCellNum; k++)
						{
							if (row1.GetCell(k) != null)
							{
								cellValue[k] = SensitiveWordController.GetCellValue(row1.GetCell(k));
							}
						}
					}
					dataTable1.Rows.Add(cellValue);
				}
				dataTable = dataTable1;
			}
			return dataTable;
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ImportExcel()
		{
			if (base.Request.Files.Count == 0)
			{
				Result result = new Result()
				{
					success = false,
					msg = "请上传文件！"
				};
				return Json(result);
			}
			int num = 0;
			HttpPostedFileBase item = base.Request.Files[num];
			if (item == null || item.ContentLength <= 0)
			{
				Result result1 = new Result()
				{
					success = false,
					msg = "上传文件格式不正确！"
				};
				return Json(result1);
			}
			Random random = new Random();
			DateTime now = DateTime.Now;
			string str = string.Concat(now.ToString("yyyyMMddHHmmssffffff"), num, Path.GetExtension(item.FileName));
			string str1 = Server.MapPath("~/temp/");
			if (!Directory.Exists(str1))
			{
				Directory.CreateDirectory(str1);
			}
			string str2 = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "/temp/");
			item.SaveAs(Path.Combine(str2, str));
			DataTable data = GetData(Path.Combine(str2, str));
			ISensitiveWordService sensitiveWordService = ServiceHelper.Create<ISensitiveWordService>();
			foreach (DataRow row in data.Rows)
			{
				if (sensitiveWordService.ExistSensitiveWord(row[1].ToString()))
				{
					continue;
				}
				SensitiveWordsInfo sensitiveWordsInfo = new SensitiveWordsInfo()
				{
					CategoryName = row[0].ToString(),
					SensitiveWord = row[1].ToString()
				};
				sensitiveWordService.AddSensitiveWord(sensitiveWordsInfo);
			}
			Result result2 = new Result()
			{
				success = true,
				msg = "导入成功！"
			};
			return Json(result2);
		}

		[UnAuthorize]
		public JsonResult List(int page, string keywords, int rows, string category)
		{
			ISensitiveWordService sensitiveWordService = ServiceHelper.Create<ISensitiveWordService>();
			SensitiveWordQuery sensitiveWordQuery = new SensitiveWordQuery()
			{
				SensitiveWord = keywords,
				CategoryName = category,
				PageNo = page,
				PageSize = rows
			};
			PageModel<SensitiveWordsInfo> sensitiveWords = sensitiveWordService.GetSensitiveWords(sensitiveWordQuery);
			var list = 
				from item in sensitiveWords.Models.ToList()
				select new { Id = item.Id, SensitiveWord = item.SensitiveWord, CategoryName = item.CategoryName };
			return Json(new { rows = list, total = sensitiveWords.Total });
		}

		public ActionResult Management()
		{
			return View();
		}
	}
}