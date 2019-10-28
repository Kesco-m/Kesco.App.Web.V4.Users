using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.Users
{
    /// <summary>
    /// Класс страницы 
    /// </summary>
    public partial class Location : Page
    {
        private int Id { get; set; }
        private string IdLoc { get; set; }
        public override string HelpUrl { get; set; }

        private string control { get; set; }
        private string callbackkey { get; set; }
        private bool multiReturn { get; set; }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Изменение рабочего места
            Title = Resx.GetString("Users_lblWorkplaceChange");

            control = Request.QueryString["control"];
            callbackkey = Request.QueryString["callbackkey"];
            multiReturn = Request.QueryString["return"] == "2";

            if (Request.QueryString["id"] != null)
            {
                Id = int.Parse(Request.QueryString["id"]);
            }
            else
            {
                Response.Write("id not found");
                Response.End();
            }

            efWorkPlace.BeforeSearch += Location_BeforeSearch;

            if (Request.QueryString["idLoc"] != null)
            {
                IdLoc = efWorkPlace.Value = Request.QueryString["idLoc"];
            }

            efChanged.ChangedByID = null;
            var dt = DBManager.GetData(SQLQueries.SELECT_РабочееМестоСотрудника, Config.DS_user, CommandType.Text,
                new Dictionary<string, object> { { "@КодСотрудника", Id }, { "@КодРасположения", efWorkPlace.Value } });
            if (dt != null && dt.Rows.Count != 0)
            {
                efChanged.SetChangeDateTime = Convert.ToDateTime(dt.Rows[0]["Изменено"].ToString());
                efChanged.ChangedByID = Convert.ToInt32(dt.Rows[0]["Изменил"].ToString());
                efChanged.Flush();
            }

        }

        /// <summary>
        ///     Обработчик события установки фильтра поиска расположения
        /// </summary>
        /// <param name="sender"></param>
        protected void Location_BeforeSearch(object sender)
        {
            efWorkPlace.Filter.WorkPlace.Value = "1,3,4";
            efWorkPlace.Filter.OnHand.Enabled = false;
            efWorkPlace.Filter.OnRepair.Enabled = false;
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
                var btnReCheck = MenuButtons.Find(btn => btn.ID == "btnReCheck");
                RemoveMenuButton(btnReCheck);
                var btnRefresh = MenuButtons.Find(btn => btn.ID == "btnRefresh");
                RemoveMenuButton(btnRefresh);
                var btnApply = MenuButtons.Find(btn => btn.ID == "btnApply");
                RemoveMenuButton(btnApply);

                var btnSave = MenuButtons.Find(btn => btn.ID == "btnSave");
                btnSave.Title = Resx.GetString("Users_cmdSaveDescription"); 

                 var btnClear = new Button
                {
                    ID = "btnDelete",
                    V4Page = this,
                    Text = Resx.GetString("cmdDelete"),
                    Title = Resx.GetString("Users_lblDeleteWorkPlace"),
                    IconJQueryUI = ButtonIconsEnum.Delete,
                    Width = 105,
                    OnClick = "cmdasync('cmd','DeleteAsc');"
                };

                AddMenuButton(btnClear);


                if (string.IsNullOrEmpty(IdLoc))
                {
                    var btnDelete = MenuButtons.Find(btn => btn.ID == "btnDelete");
                    RemoveMenuButton(btnDelete);
                }

                RenderButtons(w);

                return w.ToString();
            }
        }

        /// <summary>
        ///     Обработка клиентских команд
        /// </summary>
        /// <param name="cmd">Команды</param>
        /// <param name="param">Параметры</param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
			switch (cmd) {
				case "Save":
					Save();
					break;
                case "SaveWithOutCheck":
			        Save(false);
			        break;
                case "SaveConfirm":
                    SaveConfirm();
                    break;
				case "DeleteAsc":
					DeleteAsc();
					break;
				case "Delete":
					Delete();
					break;
                case "CloseForm":
                    CloseForm();
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
			}
		}

        private void SaveConfirm()
        {
            if (!string.IsNullOrEmpty(IdLoc) && IdLoc.Equals(efWorkPlace.Value))
            {
                //JS.Write("window.returnValue=0; window.close();");
                ReturnFromDialog("0");
                return;
            }

            if (string.IsNullOrEmpty(IdLoc))
            {
                var sqlParams = new Dictionary<string, object>
                {
                    { "@КодСотрудника", Id },
                    { "@КодРасположения",  efWorkPlace.Value}
                };

                try
                {
                    DBManager.ExecuteNonQuery(SQLQueries.INSERT_РабочееМестоСотруднику, CommandType.Text, Config.DS_user, sqlParams);
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Error", MessageStatus.Error);
                    return;
                }

                //JS.Write("window.returnValue=1; window.close();");
                ReturnFromDialog("1");
                //JS.Write("window.close();");
                return;
            }
            else
            {
                var sqlParams = new Dictionary<string, object>
                {
                    { "@КодСотрудника", Id },
                    { "@КодРасположения_Old",  IdLoc},
                    { "@КодРасположения_New",  efWorkPlace.Value}
                };

                try
                {
                    DBManager.ExecuteNonQuery(SQLQueries.UPDATE_РабочееМестоСотруднику, CommandType.Text, Config.DS_user, sqlParams);
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Error", MessageStatus.Error);
                    return;
                }
            }

            // Если есть оборудование откроем форму перемещения оборудования
            var dt = DBManager.GetData(SQLQueries.SELECT_CHECK_ОборудованиеСотрудникаВРасположении, Config.DS_user, CommandType.Text,
                new Dictionary<string, object> { { "@КодСотрудника", Id }, { "@КодРасположения", IdLoc } });
            if (dt != null && dt.Rows.Count > 0)
            {
                JS.Write("ResizeDialog(600,1000);");
                JS.Write("location_LocType('{0}','{1}','{2}','{3}','{4}');", Resx.GetString("Users_EmployeeEquipmentMove"), IDPage, IdLoc, efWorkPlace.Value, Id);
                JS.Write("resize();");
            }
            else
            {
                ReturnFromDialog("1");

                //JS.Write("window.returnValue=1; window.close();");
                //JS.Write("window.opener.OpenLocCallback(); window.close();");
                //JS.Write("window.close();");
            }
        }

        private string CheckSaveWorkPlace(string workPlaceID, int employeeID, out bool error)
        {
            error = false;
            var leaveGroup = false;
            var joinGroup = false;
            var joinCowork = false;
            var newGroupName = "";
            var oldGroupName = "";
            var employeeFIO = "";
            var fioList = new List<string>();

            string msg = "";

            var curEmpl = new Employee(employeeID.ToString());
            var curEmplIsCommon = false;
            if (curEmpl.Unavailable) return "";
            
            employeeFIO = curEmpl.Name;
            if (!string.IsNullOrEmpty(curEmpl.CommonEmployeeID))
            {
                var comEmpl = new Employee(curEmpl.CommonEmployeeID);
                if (!comEmpl.Unavailable)
                {
                    oldGroupName = comEmpl.FullName;
                    curEmplIsCommon = true;
                }
            }

            //Находим всех сотрудников на рабочем месте
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодСотрудника", employeeID },
                { "@КодРасположения",  workPlaceID}
            };

            var dt = DBManager.GetData(SQLQueries.SELECT_РаботающиеСотрудникиМестаВРасположении, Config.DS_user, CommandType.Text, sqlParams);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow employee in dt.Rows)
                {
                    if (employee["Состояние"].ToString() == "0" && String.IsNullOrEmpty(employee["КодЛица"].ToString()))
                    {
                        newGroupName = employee["Сотрудник"].ToString();
                    }
                    else if (employee["РабочееМесто"].ToString() == "1")
                    {
                        fioList.Add(employee["Сотрудник"].ToString());
                    }
                }
            }

            if (curEmplIsCommon)
            {
                if (oldGroupName != newGroupName)
                {
                    // Вы пытаетесь назначить группе {0} рабочее место, на котором уже работает группа {1}.
                    // Выполнить данную операцию невозможно!
                    msg = String.Format(Resx.GetString("Users_msgSetWorkPlaceToGroup"), oldGroupName, newGroupName);
                    error = true;
                    return msg;
                }

                if (fioList.Count > 0)
                {
                    // На рабочем месте работают сотрудники, которые не являются членами группы {0}.
                    // Вы подтверждаете, что сотрудники {1} являются членами группы посменной работы {0}?
                    msg = String.Format(Resx.GetString("Users_msgNoGroupEmployeeOnWorkPlace"), oldGroupName, String.Join(" ,", fioList.ToArray()));
                    return msg;
                }

                return "";
            }

            //Сотрудник будет назначен на посменную работу
            if (fioList.Count != 0 && String.IsNullOrEmpty(newGroupName))
                joinCowork = true;

            //Сотрудник покидает группу
            if (!String.IsNullOrEmpty(oldGroupName) && oldGroupName != newGroupName)
                leaveGroup = true;

            //Сотрудник входит в новую группу
            if (!String.IsNullOrEmpty(newGroupName) && newGroupName != oldGroupName)
                joinGroup = true;

            if (leaveGroup && joinCowork)
            {
                // {0} является членом группы {1}, но вы снимаете его с рабочего места группы, и назначаете на рабочее место, на котором посменно работают {2}.
                // Вы подтверждаете, что {0} не является членом группы {1} и работает посменно с {2}?
                msg = String.Format(Resx.GetString("Users_msgSetGroupEmployeeOnShifts"), employeeFIO, oldGroupName, String.Join(" ,", fioList.ToArray()));
            }

            if (leaveGroup && joinGroup)
            {
                var wp = DBManager.GetData(SQLQueries.SELECT_РабочиеМестаСотрудника, Config.DS_user, CommandType.Text, new Dictionary<string, object>{{ "@КодСотрудника", employeeID }});

                if (wp.Rows.Count > 1)
                {
                    error = true;
                    // {0} является членом группы {1}, но вы назначаете на рабочее место группы {2}.
                    // Сотрудник не может одновременно быть членом двух групп посменной работы.
                    msg = String.Format(Resx.GetString("Users_msgSetGroupEmployeeOnShifts"), employeeFIO, oldGroupName, newGroupName);
                }
                else
                {
                    // {0} является членом группы {1}, но вы снимаете его с рабочего места группы, и назначаете на рабочее место группы {2}. Вы подтверждаете, что {0} не является членом группы {1} и является членом группы {2}?
                    msg = String.Format(Resx.GetString("Users_msgChangeGroupEmployee"), employeeFIO, oldGroupName, newGroupName);
                }
            }

            if (leaveGroup && !joinCowork && !joinGroup)
            {
                // {0} является членом группы {1}, но вы снимаете его с рабочего места группы. Вы подтверждаете, что {0} больше не вляется членом группы {1}?
                msg = String.Format(Resx.GetString("Users_msgRemoveEmployeeFromGroup"), employeeFIO, oldGroupName);
            }

            if (!leaveGroup && joinCowork)
            {
                // Вы назначаете {0} на рабочее место, на котором посменно работают {1}. Вы подтверждаете, что {0} работает посменно с {1}?
                msg = String.Format(Resx.GetString("Users_msgSetEmployeeOnShift"), employeeFIO, String.Join(" ,", fioList.ToArray()));
            }
            
            if (!leaveGroup && joinGroup)
            {
                // Вы назначаете {0} на рабочее место группы {1}. Вы подтверждаете, что {0} является членом группы {1}?
                msg = String.Format(Resx.GetString("Users_msgSetEmployeeOnGroup"), employeeFIO, newGroupName);
            }

            return msg;
        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <param name="SaveWithCheck"></param>
		private void Save(bool SaveWithCheck = true)
		{
		    var msg = "";

            if (efWorkPlace.Value == "")
            {
                // Для сохранения необходимо выбрать рабочее место.
                ShowMessage(Resx.GetString("Users_lblSelectWorkplaceToSave"), efWorkPlace);
				return;
			}

            if (SaveWithCheck)
            {
                var error = false;
                msg = CheckSaveWorkPlace(efWorkPlace.Value, Id, out error);

                if (error)
                {
                    ShowMessage(msg, "Error", MessageStatus.Error);
                    return;
                }
            }

            if (String.IsNullOrEmpty(msg))
            {
                SaveConfirm();
            }
            else
            {
                ShowConfirm(msg, "cmdasync('cmd', 'SaveWithOutCheck')", null);
            }
        }

        /// <summary>
        /// Удаление рабочего места
        /// </summary>
		private void DeleteAsc() {
            var msg = "";
		    var employeeFIO = "";

            var dt1 = DBManager.GetData(SQLQueries.SELECT_CHECK_СотрудникВГруппе, Config.DS_user, CommandType.Text, 
                new Dictionary<string, object> { { "@КодСотрудника", Id } });
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                employeeFIO = dt1.Rows[0]["ФИО"].ToString();

                var dt2 = DBManager.GetData(SQLQueries.SELECT_CHECK_ДругиеРабочиеМеста, Config.DS_user, CommandType.Text, 
                    new Dictionary<string, object> { { "@КодСотрудника", Id }, { "@КодРасположения", efWorkPlace.Value } });
                if (dt2 != null && dt2.Rows.Count == 0)
                {
                    var dt3 = DBManager.GetData(SQLQueries.SELECT_FIO_ОбщегоСотрудника, Config.DS_user, CommandType.Text,
                        new Dictionary<string, object> { { "@КодСотрудника", Id } });

                    if (dt3 != null && dt3.Rows.Count > 0)
                    {
                        // {0} является членом группы {1}, но вы снимаете его с рабочего места группы. Вы подтверждаете, что {0} больше не вляется членом группы {1}?
                        msg = String.Format(Resx.GetString("Users_msgRemoveEmployeeFromGroup"), employeeFIO, dt3.Rows[0]["ФИО"]);
                    }
                }
            }
            
            msg += Resx.GetString("Users_msgConfirmDelete");
            ShowConfirm(msg, "cmdasync('cmd','Delete');", null);
		}

        /// <summary>
        /// Удаление рабочего места сотрудника
        /// </summary>
		private void Delete() {
            var sqlParams = new Dictionary<string, object>
            {
                { "@КодСотрудника", Id },
                { "@КодРасположения",  string.IsNullOrEmpty(IdLoc) ? efWorkPlace.Value : IdLoc}
            };

            try
            {
                DBManager.ExecuteNonQuery(SQLQueries.DELETE_РабочееМестоСотруднику, CommandType.Text, Config.DS_user, sqlParams);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "Error", MessageStatus.Error);
            }

            ReturnFromDialog("1");
            //JS.Write("window.returnValue=1; window.close();");
            //JS.Write("window.opener.OpenLocCallback(); window.close();");
            //JS.Write("window.close();");

        }

        private void CloseForm()
        {
            JS.Write("location_Close('{0}','');", IDPage);
            //JS.Write("window.opener.OpenLocCallback(); window.close();");
            JS.Write("window.close();");
        }

        protected void ReturnFromDialog(string id)
        {
/*
 ```````````if (Request.QueryString["mvc"] == "1" || Request.QueryString["mvc"] == "4")
            {
*/
                JS.Write("var result = [];");
                JS.Write("result[result.length] = {{ value: '{0}', label:\"\"}};", id);
                JS.Write("returnMvcDialogResult(result, false, true,'{0}','{1}','{2}');", control, callbackkey, multiReturn);
/*            }
            else
            {
                JS.Write("try {");
                JS.Write("window.returnValue={0};", id);
                JS.Write("} catch(e){}");

                JS.Write(@"
var version = parseFloat(navigator.appVersion.split('MSIE')[1]);
if (version<7) window.opener=this;
else window.open('','_parent','');
window.close();");
            }
*/
        }


    }
}