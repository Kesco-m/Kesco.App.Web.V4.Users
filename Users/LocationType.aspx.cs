using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.Users
{
	public partial class LocationType : Page
    {
        private string Loc1 { get; set; }
        private string Loc2 { get; set; }
        private string User { get; set; }

        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Перемещение оборудования сотрудника.
            Title = Resx.GetString("Users_EmployeeEquipmentMove");
            efEmployee.Changed += Employee_Changed;

			if (!V4IsPostBack) {
                Loc1 = Request.QueryString["loc1"];
                Loc2 = efLocation_New.Value = Request.QueryString["loc2"];
                User = efEmployee.Value = Request.QueryString["user"];

                if (Loc1.Length == 0) {
                    // Не передан исходный код расположения
					Response.Write(Resx.GetString("Users_NoLocationSource"));
					Response.End();
				}
                else
                {
                    efLocation_Old.Value = Loc1;
                }

                if (Loc2.Length == 0)
                {
                    // Не передан новый код расположения
                    Response.Write(Resx.GetString("Users_NoEmployeeDestination"));
                    Response.End();
                }
                else
                {
                    efLocation_Old.Value = Loc1;
                }

                if (User.Length == 0) {
                    // Не передан код сотрудника
					Response.Write(Resx.GetString("Users_NoEmployee"));
					Response.End();
				}
                else
                {
                    efEmployee.Value = User;
                }

                efLocation_Old.BeforeSearch += LocationOld_BeforeSearch;
                efLocation_New.BeforeSearch += LocationNew_BeforeSearch;
			}
		}

        /// <summary>
        ///     Подготовка данных для отрисовки заголовка страницы(панели с кнопками)
        /// </summary>
        /// <returns></returns>
        protected string RenderHeader()
        {
            using (var w = new StringWriter())
            {
                var btnEdit = MenuButtons.Find(btn => btn.ID == "btnEdit");
                RemoveMenuButton(btnEdit);
                var btnSave = MenuButtons.Find(btn => btn.ID == "btnSave");
                RemoveMenuButton(btnSave);
                var btnApply = MenuButtons.Find(btn => btn.ID == "btnApply");
                RemoveMenuButton(btnApply);
                var btnReCheck = MenuButtons.Find(btn => btn.ID == "btnReCheck");
                RemoveMenuButton(btnReCheck);
                var btnRefresh = MenuButtons.Find(btn => btn.ID == "btnRefresh");
                RemoveMenuButton(btnRefresh);

                var btnMove = new Button
                {
                    ID = "btnMove",
                    V4Page = this,
                    Text = Resx.GetString("Users_lblMoveEquipment"),
                    Title = Resx.GetString("Users_lblMoveEquipment"),
                    IconJQueryUI = ButtonIconsEnum.Copy,
                    Width = 205,
                    OnClick = "Move();"
                };
                AddMenuButton(btnMove);

                RenderButtons(w);

                return w.ToString();
            }
        }

        /// <summary>
        ///     Обработчик события установки фильтра поиска расположения (откуда)
        /// </summary>
        /// <param name="sender"></param>
        protected void LocationOld_BeforeSearch(object sender)
        {
            //efLocation_Old.Filter.IDs.Add(Loc1);
            efLocation_New.Filter.WorkPlace.Value = "1,3,4";
        }

        /// <summary>
        ///     Обработчик события установки фильтра поиска расположения (куда)
        /// </summary>
        /// <param name="sender"></param>
        protected void LocationNew_BeforeSearch(object sender)
        {
            efLocation_New.Filter.IDs.Add(Loc2);
            efLocation_New.Filter.WorkPlace.Value = "1,3,4";
        }

        /// <summary>
        ///     Обработка клиентских команд
        /// </summary>
        /// <param name="cmd">Команды</param>
        /// <param name="param">Параметры</param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        { 
			switch (cmd) {
				case "RefreshEquipment":
					RefreshEquipment();
					break;

				case "Move":
					Move(param["id1"], param["id2"]);
					break;

				default:
					base.ProcessCommand(cmd, param);
					break;
			}
		}

        /// <summary>
        /// Обновление списка оборудования
        /// </summary>
		public void RefreshEquipment()
        {
			var w = new StringWriter();
			RenderEquipment(w);
			JS.Write("document.all('Equipment').innerHTML='{0}';", HttpUtility.JavaScriptStringEncode(w.ToString(), false));
		}

        /// <summary>
        /// Отрисовка списка оборудования
        /// </summary>
        /// <param name="w"></param>
        public void RenderEquipment(TextWriter w)
        {
            var sqlParams1 = new Dictionary<string, object>
            {
                {"@КодСотрудника",  efEmployee.Value},
                {"@КодРасположения", Loc1}
            };

            var dt = DBManager.GetData(SQLQueries.SELECT_ОборудованиеСотрудникаВРасположении, Config.DS_user, CommandType.Text, sqlParams1);

            if (dt.Rows.Count != 0)
            {
                //w.Write("<table id='Equipment1' width='100%' class='grid' style='table-layout: fixed;'>");
                //w.Write("<tr class='floatThead-col'>");

                w.Write("<b>{0}</b>", Resx.GetString("Users_lblEquipmentMovedToNewWorkplace"));
                w.Write("<table id='Equipment1' width='100%' class='grid' style='border-collapse:collapse; background-color: white'>");
                w.Write("<tr class='gridHeader'>");
                w.Write("<td width='1%'><input type=checkbox onclick='MarkAll()'></td>");
                w.Write("<td>{0}</td>", Resx.GetString("sCode"));
                w.Write("<td>{0}</td>", Resx.GetString("TTN_lblType"));
                w.Write("<td>{0}</td>", Resx.GetString("Users_lblModele"));
                w.Write("<td>{0}</td>", Resx.GetString("lblPosition"));
                w.Write("</tr>");

                foreach (DataRow r in dt.Rows)
                {
                    w.Write("<tr>");
                    w.Write("<td><input type=checkbox></td>");
                    w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[0].ToString()));
                    w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[1].ToString()));
                    w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[2].ToString()));
                    w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[3].ToString()));
                    w.Write("</tr>");
                }

                w.Write("</table>");
                w.Write("<br>");

                if (Loc2.Length > 0)
                {
                    var sqlParams2 = new Dictionary<string, object>
                    {
                        {"@КодСотрудника",  efEmployee.Value},
                        {"@КодРасположения", Loc2}
                    };

                    var odt = DBManager.GetData(SQLQueries.SELECT_ДругоеОборудованиеВРасположении, Config.DS_user, CommandType.Text, sqlParams2);

                    if (odt.Rows.Count != 0)
                    {
                        
                        w.Write("<b>{0}</b>", Resx.GetString("Users_lblEquipmentEmployeeAtNewWorkplace"));
                        w.Write("<table id='Equipment2' width='100%' class='Grid8' style='border-collapse:collapse; background-color: white'>");
                        w.Write("<tr class='GridHeader'>");
                        w.Write("<td width='1%'><input type=checkbox onclick='MarkAll()'></td>");
                        w.Write("<td>{0}</td>", Resx.GetString("sCode"));
                        w.Write("<td>{0}</td>", Resx.GetString("TTN_lblType"));
                        w.Write("<td>{0}</td>", Resx.GetString("Users_lblModele"));
                        w.Write("<td>{0}</td>", Resx.GetString("Users_lblPreviousResponsible"));
                        w.Write("</tr>");

                        foreach (DataRow r in odt.Rows)
                        {
                            w.Write("<tr>");
                            w.Write("<td><input type=checkbox></td>");
                            w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[0].ToString()));
                            w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[1].ToString()));
                            w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[2].ToString()));
                            w.Write("<td>{0}</td>", HttpUtility.HtmlEncode(r[3].ToString()));
                            w.Write("</tr>");
                        }

                        w.Write("</table>");
                    }
                }
            }
        }

        /// <summary>
        /// Перемещение оборудования
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        private void Move(string id1, string id2)
        {
            try
            {
                if (id1.Length > 0)
                {
                    var sqlParams = new Dictionary<string, object>
                    {
                        {"@КодСотрудника", efEmployee.Value},
                        {"@КодРасположения", Loc2},
                        {"@СписокОборудования", id1}
                    };
                    DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ПеремещениеОборудования1, CommandType.Text, Config.DS_user, sqlParams);
                }

                if (id2.Length > 0)
                {
                    var sqlParams = new Dictionary<string, object>
                    {
                        {"@КодСотрудника", efEmployee.Value},
                        {"@Время1", DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss")},
                        {"@Время2", DateTime.UtcNow.AddSeconds(1).ToString("yyyyMMdd HH:mm:ss")},
                        {"@СписокОборудования", id2}
                    };
                    DBManager.ExecuteNonQuery(SQLQueries.UPDATE_ПеремещениеОборудования2, CommandType.Text, Config.DS_user, sqlParams);
                }
                RefreshEquipment();

                JS.Write("parent.location_Records_Save();");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "Error", MessageStatus.Error);
            }
        }

        /// <summary>
        ///     Событие, отслеживающее изменение сотрудника
        /// </summary>
        /// <param name="sender">Контрол</param>
        /// <param name="e">Аргументы</param>
        protected void Employee_Changed(object sender, ProperyChangedEventArgs e)
        {
            RefreshEquipment();
        }

	}
}