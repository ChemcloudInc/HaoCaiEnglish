using Himall.Core.Helper;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class InstallerController : Controller
	{
		private IList<string> errorMsgs;

		private int usernameMinLength = 3;

		private int usernameMaxLength = 20;

		private string usernameRegex = "[一-_a-zA-Z0-9]+[一-_a-zA-Z0-9]*";

		private int passwordMaxLength = 16;

		private int passwordMinLength = 6;

		private string _dbServer;

		private string _dbPort;

		private string _dbName;

		private string _dbLoginName;

		private string _dbPwd;

		private string _siteName;

		private string _siteAdminName;

		private string _sitePwd;

		private string _sitePwd2;

		private string _shopName;

		private string _shopAdminName;

		private string _shopPwd;

		private string _shopPwd2;

		public InstallerController()
		{
		}

		private bool AddDemoData(out string errorMsg)
		{
			bool flag;
			string str = base.Request.MapPath("/SqlScripts/SiteDemo.zh-CN.sql");
			if (!System.IO.File.Exists(str))
			{
				errorMsg = "没有找到演示数据文件-SiteDemo.Sql";
				return false;
			}
			try
			{
				flag = ExecuteScriptFile(str, out errorMsg);
			}
			catch
			{
				errorMsg = "演示数据创建错误";
				flag = false;
			}
			return flag;
		}

		private bool AddInitData(out string errorMsg)
		{
			bool flag;
			string str = base.Request.MapPath("/SqlScripts/Storage");
			string str1 = base.Request.MapPath("/Storage");
			try
			{
				Directory.Move(str, str1);
			}
			catch
			{
			}
			string str2 = base.Request.MapPath("/SqlScripts/SiteInitData.zh-CN.Sql");
			if (!System.IO.File.Exists(str2))
			{
				errorMsg = "没有找到初始化数据文件-SiteInitData.Sql";
				return false;
			}
			try
			{
				flag = ExecuteScriptFile(str2, out errorMsg);
			}
			catch
			{
				errorMsg = "初始化数据创建错误";
				flag = false;
			}
			return flag;
		}

		public ActionResult Agreement()
		{
			return View();
		}

		public ActionResult Completed()
		{
			return View();
		}

		public ActionResult Configuration()
		{
			ViewBag.IsDebug = GetSolutionDebugState();
			return View();
		}

		private bool CreateAdministrator(out string errorMsg)
		{
			bool flag;
			DbConnection dbConnection = null;
			DbTransaction dbTransaction = null;
			try
			{
				SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                dbConnection = sqlConnection;
                using (sqlConnection)
				{
					dbConnection.Open();
					DbCommand dbCommand = dbConnection.CreateCommand();
					dbTransaction = dbConnection.BeginTransaction();
					dbCommand.Connection = dbConnection;
					dbCommand.Transaction = dbTransaction;
					dbCommand.CommandType = CommandType.Text;
					string str = Guid.NewGuid().ToString();
					string passwrodWithTwiceEncode = GetPasswrodWithTwiceEncode(_sitePwd, str);
					string str1 = Guid.NewGuid().ToString();
					string passwrodWithTwiceEncode1 = GetPasswrodWithTwiceEncode(_shopPwd, str1);
					dbCommand.CommandText = "SELECT top 1 Id FROM Himall_Shops";
					long num = (long)dbCommand.ExecuteScalar();
					dbCommand.Parameters.Clear();
					dbCommand.CommandText = "INSERT INTO Himall_Managers  (shopId, RoleId, UserName, Password, PasswordSalt, CreateDate) VALUES (@shopId, 0, @userName, @Password, @PasswordSalt,@CreateDate )";
					dbCommand.Parameters.Add(new SqlParameter("@shopId", num));
					dbCommand.Parameters.Add(new SqlParameter("@userName", _shopAdminName));
					dbCommand.Parameters.Add(new SqlParameter("@Password", passwrodWithTwiceEncode1));
					dbCommand.Parameters.Add(new SqlParameter("@PasswordSalt", str1));
					dbCommand.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now));
					dbCommand.ExecuteNonQuery();
					dbCommand.Parameters.Clear();
					dbCommand.CommandText = "INSERT INTO Himall_Managers  (shopId, RoleId, UserName, Password, PasswordSalt, CreateDate) VALUES (0, 0, @userName, @Password, @PasswordSalt,@CreateDate )";
					dbCommand.Parameters.Add(new SqlParameter("@userName", _siteAdminName));
					dbCommand.Parameters.Add(new SqlParameter("@Password", passwrodWithTwiceEncode));
					dbCommand.Parameters.Add(new SqlParameter("@PasswordSalt", str));
					dbCommand.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now));
					dbCommand.ExecuteNonQuery();
					dbCommand.Parameters.Clear();
                    dbCommand.CommandText = "INSERT INTO Himall_Members  (UserName, Password, PasswordSalt,TopRegionId,RegionId,OrderNumber,Disabled,Points,Expenditure,CreateDate,LastLoginDate,ParentSellerId) VALUES (@userName, @Password, @PasswordSalt,0,0,0,0,0,0.00,@CreateDate,@LastLoginDate,0)";
					dbCommand.Parameters.Add(new SqlParameter("@userName", _shopAdminName));
					dbCommand.Parameters.Add(new SqlParameter("@Password", passwrodWithTwiceEncode1));
					dbCommand.Parameters.Add(new SqlParameter("@PasswordSalt", str1));
					dbCommand.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now));
					dbCommand.Parameters.Add(new SqlParameter("@LastLoginDate", DateTime.Now));
					dbCommand.ExecuteNonQuery();
					dbCommand.Parameters.Clear();
                    dbCommand.CommandText = "update Himall_SiteSettings set Value=@SiteName WHERE [Key] ='SiteName'";
					dbCommand.Parameters.Add(new SqlParameter("@SiteName", _siteName));
					dbCommand.ExecuteNonQuery();
					dbTransaction.Commit();
					dbConnection.Close();
				}
				errorMsg = null;
				flag = true;
			}
			catch (SqlException sqlException)
			{
				errorMsg = sqlException.Message;
				if (dbTransaction != null)
				{
					try
					{
						dbTransaction.Rollback();
					}
					catch (Exception exception)
					{
						errorMsg = exception.Message;
					}
				}
				if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
				{
					dbConnection.Close();
					dbConnection.Dispose();
				}
				flag = false;
			}
			return flag;
		}

		private bool CreateDataSchema(out string errorMsg)
		{
			bool flag;
			string str = base.Request.MapPath("/SqlScripts/Schema.sql");
			if (!System.IO.File.Exists(str))
			{
				errorMsg = "没有找到数据库架构文件-Schema.sql";
				return false;
			}
			try
			{
				flag = ExecuteScriptFile(str, out errorMsg);
			}
			catch
			{
				errorMsg = "数据架构创建错误";
				flag = false;
			}
			return flag;
		}

		private bool CreateDtabase(out string msg)
		{
			bool flag;
			string str = string.Format("server={0};user id={1};password={2};persistsecurityinfo=True;", _dbServer, _dbLoginName, _dbPwd);
			using (DbConnection mySqlConnection = new SqlConnection(str))
			{
				msg = "";
				DbCommand dbCommand = mySqlConnection.CreateCommand();
				dbCommand.CommandType = CommandType.Text;
				dbCommand.CommandText = string.Concat("CREATE DATABASE ", _dbName);
				if (_dbName.IndexOf('.') < 0)
				{
					try
					{
						try
						{
							mySqlConnection.Open();
							dbCommand.ExecuteNonQuery();
						}
						catch (Exception exception)
						{
							msg = "数据库创建失败";
							flag = false;
							return flag;
						}
					}
					finally
					{
						mySqlConnection.Close();
					}
					return true;
				}
				else
				{
					msg = "数据库名不能含有字符\".\"";
					flag = false;
				}
			}
			return flag;
		}

		private bool ExecuteScriptFile(string pathToScriptFile, out string errorMsg)
		{
			StreamReader streamReader = null;
			DbConnection dbConnection = null;
			string applicationPath = base.Request.ApplicationPath;
			StreamReader streamReader1 = new StreamReader(pathToScriptFile);
			streamReader = streamReader1;
			using (streamReader1)
			{
				SqlConnection mySqlConnection = new SqlConnection(GetConnectionString());
				dbConnection = mySqlConnection;
				using (mySqlConnection)
				{
					DbCommand dbCommand = dbConnection.CreateCommand();
					dbCommand.Connection = dbConnection;
					dbCommand.CommandType = CommandType.Text;
					dbCommand.CommandTimeout = 360;
					dbConnection.Open();
					while (!streamReader.EndOfStream)
					{
						try
						{
							string str = InstallerController.NextSqlFromStream(streamReader);
							if (!string.IsNullOrEmpty(str))
							{
								dbCommand.CommandText = str.Replace("$VirsualPath$", applicationPath);
								dbCommand.ExecuteNonQuery();
							}
						}
						catch (Exception exception)
						{
							throw new Exception(exception.Message);
						}
					}
					dbConnection.Close();
				}
				streamReader.Close();
			}
			errorMsg = null;
			return true;
		}

		private bool ExecuteTest()
		{
			string str;
			bool count;
            errorMsgs = new List<string>();
			DbTransaction dbTransaction = null;
			DbConnection dbConnection = null;
			try
			{
				if (!ValidateConnectionStrings(out str))
				{
                    errorMsgs.Add(str);
				}
				else
				{
					SqlConnection mySqlConnection = new SqlConnection(GetConnectionString());
					dbConnection = mySqlConnection;
					using (mySqlConnection)
					{
						dbConnection.Open();
						DbCommand dbCommand = dbConnection.CreateCommand();
						dbTransaction = dbConnection.BeginTransaction();
						dbCommand.Connection = dbConnection;
						dbCommand.Transaction = dbTransaction;
						dbCommand.CommandText = "CREATE TABLE installTest(Test bit NULL)";
						dbCommand.ExecuteNonQuery();
						dbCommand.CommandText = "DROP TABLE installTest";
						dbCommand.ExecuteNonQuery();
						dbTransaction.Commit();
						dbConnection.Close();
					}
				}
				return errorMsgs.Count == 0;
			}
			catch (Exception exception1)
			{
                errorMsgs.Add(exception1.Message);
				if (dbTransaction != null)
				{
					try
					{
						dbTransaction.Rollback();
					}
					catch (Exception exception)
					{
                        errorMsgs.Add(exception.Message);
						count = errorMsgs.Count == 0;
						return count;
					}
				}
				if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
				{
					dbConnection.Close();
					dbConnection.Dispose();
				}
				count = errorMsgs.Count == 0;
			}
			return count;
		}

		private string GetConnectionString()
		{
			int num = 0;
			int.TryParse(_dbPort, out num);
			object[] objArray = new object[] { _dbServer, _dbName, _dbLoginName, _dbPwd };
			return string.Format("server={0};database={1};user id={2};password={3};persistsecurityinfo=True;", objArray);
		}

		private string GetEFConnectionString()
		{
			int num = 0;
			int.TryParse(_dbPort, out num);
			object[] objArray = new object[] { _dbServer, null, null, null, null };
			objArray[1] = (num == 0 ? "" : string.Concat(",", _dbPort));
			objArray[2] = _dbLoginName;
			objArray[3] = _dbPwd;
			objArray[4] = _dbName;
            return string.Format("metadata=res://*/Entities.csdl|res://*/Entities.ssdl|res://*/Entities.msl;provider=System.Data.SqlClient;provider connection string=\"data source={0}{1};initial catalog={4};persist security info=True;uid={2};Password={3};MultipleActiveResultSets=True;App=EntityFramework\";", objArray);
		}

		private string GetPasswrodWithTwiceEncode(string password, string salt)
		{
			string str = SecureHelper.MD5(password);
			return SecureHelper.MD5(string.Concat(str, salt));
		}

		private bool GetSolutionDebugState()
		{
			return false;
		}

		private bool IsInstalled()
		{
			string item = ConfigurationManager.AppSettings["IsInstalled"];
			if (item == null)
			{
				return true;
			}
			return bool.Parse(item);
		}

		private static string NextSqlFromStream(StreamReader reader)
		{
			string i;
			StringBuilder stringBuilder = new StringBuilder();
			for (i = reader.ReadLine().Trim(); !reader.EndOfStream && string.Compare(i, "GO", true, CultureInfo.InvariantCulture) != 0; i = reader.ReadLine())
			{
				stringBuilder.Append(string.Concat(i, Environment.NewLine));
			}
			if (string.Compare(i, "GO", true, CultureInfo.InvariantCulture) != 0)
			{
				stringBuilder.Append(string.Concat(i, Environment.NewLine));
			}
			return stringBuilder.ToString();
		}

		private bool SaveConfig(out string errorMsg)
		{
			bool flag;
			try
			{
				Configuration eFConnectionString = WebConfigurationManager.OpenWebConfiguration(base.Request.ApplicationPath);
				eFConnectionString.AppSettings.Settings["IsInstalled"].Value = "true";
				eFConnectionString.ConnectionStrings.ConnectionStrings["Entities"].ConnectionString = GetEFConnectionString();
				eFConnectionString.Save();
				errorMsg = null;
				flag = true;
			}
			catch (Exception exception)
			{
				errorMsg = exception.Message;
				flag = false;
			}
			return flag;
		}

		[HttpPost]
		public JsonResult SaveConfiguration(string dbServer, string dbName, string dbLoginName, string dbPwd, string siteName, string siteAdminName, string sitePwd, string sitePwd2, string shopName, string shopAdminName, string shopPwd, string shopPwd2, int installData)
		{
			if (IsInstalled())
			{
				return Json(new { successful = true, msg = "软件已经安装,不需要重新安装.", status = 0 }, JsonRequestBehavior.AllowGet);
			}
            _dbServer = dbServer;
            _dbPort = "";
            _dbName = dbName;
            _dbLoginName = dbLoginName;
            _dbPwd = dbPwd;
            _siteName = siteName;
            _siteAdminName = siteAdminName;
            _sitePwd = sitePwd;
            _sitePwd2 = sitePwd2;
            _shopName = shopName;
            _shopAdminName = shopAdminName;
            _shopPwd = shopPwd;
            _shopPwd2 = shopPwd2;
			string empty = string.Empty;
			if (!ValidateUser(out empty))
			{
				return Json(new { successful = true, errorMsg = empty }, JsonRequestBehavior.AllowGet);
			}
			if (!CreateDtabase(out empty))
			{
				return Json(new { successful = true, errorMsg = empty }, JsonRequestBehavior.AllowGet);
			}
			if (!ExecuteTest())
			{
				return Json(new { successful = true, errorMsg = "数据库链接信息有误" }, JsonRequestBehavior.AllowGet);
			}
			if (!TestPermission())
			{
				return Json(new { successful = true, errorMsg = "WEB目录读写权限不够." }, JsonRequestBehavior.AllowGet);
			}
			if (!CreateDataSchema(out empty))
			{
				return Json(new { successful = true, errorMsg = empty }, JsonRequestBehavior.AllowGet);
			}
			if (!AddInitData(out empty))
			{
				return Json(new { successful = true, errorMsg = empty }, JsonRequestBehavior.AllowGet);
			}
			if (!CreateAdministrator(out empty))
			{
				return Json(new { successful = true, errorMsg = empty }, JsonRequestBehavior.AllowGet);
			}
			if (installData == 1)
			{
				if (!AddDemoData(out empty))
				{
					return Json(new { successful = true, errorMsg = "添加演示数据失败" }, JsonRequestBehavior.AllowGet);
				}
			}
			else if (!UpdateSliderImage(out empty))
			{
				return Json(new { successful = true, errorMsg = empty }, JsonRequestBehavior.AllowGet);
			}
			if (!SaveConfig(out empty))
			{
				return Json(new { successful = true, errorMsg = empty }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { successful = true, msg = "安装成功", status = 1 }, JsonRequestBehavior.AllowGet);
		}

		private static bool TestFolder(string folderPath, out string errorMsg)
		{
			bool flag;
			try
			{
                System.IO.File.WriteAllText(folderPath, "Hi");
                System.IO.File.AppendAllText(folderPath, ",This is a test file.");
                System.IO.File.Delete(folderPath);
				errorMsg = null;
				flag = true;
			}
			catch (Exception exception)
			{
				errorMsg = exception.Message;
				flag = false;
			}
			return flag;
		}

		private bool TestPermission()
		{
			string str;
            errorMsgs = new List<string>();
			try
			{
				Configuration configuration = WebConfigurationManager.OpenWebConfiguration(base.Request.ApplicationPath);
				if (configuration.ConnectionStrings.ConnectionStrings["Entities"].ConnectionString != "none")
				{
					configuration.ConnectionStrings.ConnectionStrings["Entities"].ConnectionString = "none";
				}
				else
				{
					configuration.ConnectionStrings.ConnectionStrings["Entities"].ConnectionString = "required";
				}
				configuration.Save();
			}
			catch (Exception exception)
			{
                errorMsgs.Add(exception.Message);
			}
			if (!InstallerController.TestFolder(base.Request.MapPath(string.Concat(base.Request.ApplicationPath, "/storage/test.txt")), out str))
			{
                errorMsgs.Add(str);
			}
			return errorMsgs.Count == 0;
		}

		private bool UpdateSliderImage(out string errorMsg)
		{
			bool flag;
			DbConnection dbConnection = null;
			DbTransaction dbTransaction = null;
			try
			{
				SqlConnection mySqlConnection = new SqlConnection(GetConnectionString());
				dbConnection = mySqlConnection;
				using (mySqlConnection)
				{
					dbConnection.Open();
					DbCommand dbCommand = dbConnection.CreateCommand();
					dbTransaction = dbConnection.BeginTransaction();
					dbCommand.Connection = dbConnection;
					dbCommand.Transaction = dbTransaction;
					dbCommand.CommandType = CommandType.Text;
					string str = Guid.NewGuid().ToString();
                    GetPasswrodWithTwiceEncode(_sitePwd, str);
					string str1 = Guid.NewGuid().ToString();
                    GetPasswrodWithTwiceEncode(_shopPwd, str1);
					dbCommand.Parameters.Clear();
					dbCommand.CommandText = "update Himall_ImageAds set [ImageUrl] = @ImageUrl WHERE [Id] > 1 and [Id] < 13";
					dbCommand.Parameters.Add(new SqlParameter("@ImageUrl", "http://fpoimg.com/295x192"));
					dbCommand.ExecuteScalar();
					dbCommand.Parameters.Clear();
					dbCommand.CommandText = "update Himall_ImageAds set [ImageUrl] = @ImageUrl WHERE [Id] = 1";
					dbCommand.Parameters.Add(new SqlParameter("@ImageUrl", "http://fpoimg.com/310x70"));
					dbCommand.ExecuteScalar();
					dbCommand.Parameters.Clear();
					dbCommand.CommandText = "update Himall_ImageAds set [ImageUrl] = @ImageUrl WHERE [Id] = 13";
					dbCommand.Parameters.Add(new SqlParameter("@ImageUrl", "http://fpoimg.com/310x165"));
					dbCommand.ExecuteScalar();
					dbTransaction.Commit();
					dbConnection.Close();
				}
				errorMsg = null;
				flag = true;
			}
			catch (SqlException sqlException)
			{
				errorMsg = sqlException.Message;
				if (dbTransaction != null)
				{
					try
					{
						dbTransaction.Rollback();
					}
					catch (Exception exception)
					{
						errorMsg = exception.Message;
					}
				}
				if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
				{
					dbConnection.Close();
					dbConnection.Dispose();
				}
				flag = false;
			}
			return flag;
		}

		private bool ValidateConnectionStrings(out string msg)
		{
			msg = null;
			if (!string.IsNullOrEmpty(_dbServer) && !string.IsNullOrEmpty(_dbName) && !string.IsNullOrEmpty(_dbLoginName))
			{
				return true;
			}
			msg = "数据库连接信息不完整";
			return false;
		}

		private bool ValidateUser(out string msg)
		{
			msg = null;
			if (string.IsNullOrEmpty(_siteAdminName) || string.IsNullOrEmpty(_sitePwd) || string.IsNullOrEmpty(_sitePwd2))
			{
				msg = "管理员账号信息不完整";
				return false;
			}
			if (string.IsNullOrEmpty(_shopAdminName) || string.IsNullOrEmpty(_shopPwd) || string.IsNullOrEmpty(_shopPwd2))
			{
				msg = "店铺管理员账号信息不完整";
				return false;
			}
			if (_siteAdminName.Length > usernameMaxLength || _siteAdminName.Length < usernameMinLength)
			{
				msg = string.Format("管理员用户名的长度只能在{0}和{1}个字符之间", usernameMinLength, usernameMaxLength);
				return false;
			}
			if (_shopAdminName.Length > usernameMaxLength || _shopAdminName.Length < usernameMinLength)
			{
				msg = string.Format("店铺管理员用户名的长度只能在{0}和{1}个字符之间", usernameMinLength, usernameMaxLength);
				return false;
			}
			if (string.Compare(_siteAdminName, "anonymous", true) == 0)
			{
				msg = "不能使用anonymous作为管理员用户名";
				return false;
			}
			if (string.Compare(_shopAdminName, "anonymous", true) == 0)
			{
				msg = "不能使用anonymous作为店铺管理员用户名";
				return false;
			}
			if (!Regex.IsMatch(_siteAdminName, usernameRegex))
			{
				msg = "管理员用户名的格式不符合要求，用户名一般由字母、数字、下划线和汉字组成，且必须以汉字或字母开头";
				return false;
			}
			if (!Regex.IsMatch(_shopAdminName, usernameRegex))
			{
				msg = "店铺管理员用户名的格式不符合要求，用户名一般由字母、数字、下划线和汉字组成，且必须以汉字或字母开头";
				return false;
			}
			if (_sitePwd != _sitePwd2)
			{
				msg = "管理员登录密码两次输入不一致";
				return false;
			}
			if (_shopPwd != _shopPwd2)
			{
				msg = "店铺管理员登录密码两次输入不一致";
				return false;
			}
			if (_sitePwd.Length < passwordMinLength || _sitePwd.Length > passwordMaxLength)
			{
				msg = string.Format("管理员登录密码的长度只能在{0}和{1}个字符之间", passwordMinLength, passwordMaxLength);
				return false;
			}
			if (_shopPwd.Length >= passwordMinLength && _shopPwd.Length <= passwordMaxLength)
			{
				return true;
			}
			msg = string.Format("店铺管理员登录密码的长度只能在{0}和{1}个字符之间", passwordMinLength, passwordMaxLength);
			return false;
		}
	}
}