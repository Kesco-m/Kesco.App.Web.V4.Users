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
    /// ����� �������� 
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
        ///     ������� �������� ��������
        /// </summary>
        /// <param name="sender">������ ��������</param>
        /// <param name="e">���������</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // ��������� �������� �����
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
            var dt = DBManager.GetData(SQLQueries.SELECT_����������������������, Config.DS_user, CommandType.Text,
                new Dictionary<string, object> { { "@�������������", Id }, { "@���������������", efWorkPlace.Value } });
            if (dt != null && dt.Rows.Count != 0)
            {
                efChanged.SetChangeDateTime = Convert.ToDateTime(dt.Rows[0]["��������"].ToString());
                efChanged.ChangedByID = Convert.ToInt32(dt.Rows[0]["�������"].ToString());
                efChanged.Flush();
            }

        }

        /// <summary>
        ///     ���������� ������� ��������� ������� ������ ������������
        /// </summary>
        /// <param name="sender"></param>
        protected void Location_BeforeSearch(object sender)
        {
            efWorkPlace.Filter.WorkPlace.Value = "1,3,4";
            efWorkPlace.Filter.OnHand.Enabled = false;
            efWorkPlace.Filter.OnRepair.Enabled = false;
        }

        /// <summary>
        ///     ���������� ������ ��� ��������� ��������� ��������(������ � ��������)
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
        ///     ��������� ���������� ������
        /// </summary>
        /// <param name="cmd">�������</param>
        /// <param name="param">���������</param>
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
                    { "@�������������", Id },
                    { "@���������������",  efWorkPlace.Value}
                };

                try
                {
                    DBManager.ExecuteNonQuery(SQLQueries.INSERT_����������������������, CommandType.Text, Config.DS_user, sqlParams);
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
                    { "@�������������", Id },
                    { "@���������������_Old",  IdLoc},
                    { "@���������������_New",  efWorkPlace.Value}
                };

                try
                {
                    DBManager.ExecuteNonQuery(SQLQueries.UPDATE_����������������������, CommandType.Text, Config.DS_user, sqlParams);
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Error", MessageStatus.Error);
                    return;
                }
            }

            // ���� ���� ������������ ������� ����� ����������� ������������
            var dt = DBManager.GetData(SQLQueries.SELECT_CHECK_�����������������������������������, Config.DS_user, CommandType.Text,
                new Dictionary<string, object> { { "@�������������", Id }, { "@���������������", IdLoc } });
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

            //������� ���� ����������� �� ������� �����
            var sqlParams = new Dictionary<string, object>
            {
                { "@�������������", employeeID },
                { "@���������������",  workPlaceID}
            };

            var dt = DBManager.GetData(SQLQueries.SELECT_��������������������������������������, Config.DS_user, CommandType.Text, sqlParams);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow employee in dt.Rows)
                {
                    if (employee["���������"].ToString() == "0" && String.IsNullOrEmpty(employee["�������"].ToString()))
                    {
                        newGroupName = employee["���������"].ToString();
                    }
                    else if (employee["������������"].ToString() == "1")
                    {
                        fioList.Add(employee["���������"].ToString());
                    }
                }
            }

            if (curEmplIsCommon)
            {
                if (oldGroupName != newGroupName)
                {
                    // �� ��������� ��������� ������ {0} ������� �����, �� ������� ��� �������� ������ {1}.
                    // ��������� ������ �������� ����������!
                    msg = String.Format(Resx.GetString("Users_msgSetWorkPlaceToGroup"), oldGroupName, newGroupName);
                    error = true;
                    return msg;
                }

                if (fioList.Count > 0)
                {
                    // �� ������� ����� �������� ����������, ������� �� �������� ������� ������ {0}.
                    // �� �������������, ��� ���������� {1} �������� ������� ������ ��������� ������ {0}?
                    msg = String.Format(Resx.GetString("Users_msgNoGroupEmployeeOnWorkPlace"), oldGroupName, String.Join(" ,", fioList.ToArray()));
                    return msg;
                }

                return "";
            }

            //��������� ����� �������� �� ��������� ������
            if (fioList.Count != 0 && String.IsNullOrEmpty(newGroupName))
                joinCowork = true;

            //��������� �������� ������
            if (!String.IsNullOrEmpty(oldGroupName) && oldGroupName != newGroupName)
                leaveGroup = true;

            //��������� ������ � ����� ������
            if (!String.IsNullOrEmpty(newGroupName) && newGroupName != oldGroupName)
                joinGroup = true;

            if (leaveGroup && joinCowork)
            {
                // {0} �������� ������ ������ {1}, �� �� �������� ��� � �������� ����� ������, � ���������� �� ������� �����, �� ������� �������� �������� {2}.
                // �� �������������, ��� {0} �� �������� ������ ������ {1} � �������� �������� � {2}?
                msg = String.Format(Resx.GetString("Users_msgSetGroupEmployeeOnShifts"), employeeFIO, oldGroupName, String.Join(" ,", fioList.ToArray()));
            }

            if (leaveGroup && joinGroup)
            {
                var wp = DBManager.GetData(SQLQueries.SELECT_����������������������, Config.DS_user, CommandType.Text, new Dictionary<string, object>{{ "@�������������", employeeID }});

                if (wp.Rows.Count > 1)
                {
                    error = true;
                    // {0} �������� ������ ������ {1}, �� �� ���������� �� ������� ����� ������ {2}.
                    // ��������� �� ����� ������������ ���� ������ ���� ����� ��������� ������.
                    msg = String.Format(Resx.GetString("Users_msgSetGroupEmployeeOnShifts"), employeeFIO, oldGroupName, newGroupName);
                }
                else
                {
                    // {0} �������� ������ ������ {1}, �� �� �������� ��� � �������� ����� ������, � ���������� �� ������� ����� ������ {2}. �� �������������, ��� {0} �� �������� ������ ������ {1} � �������� ������ ������ {2}?
                    msg = String.Format(Resx.GetString("Users_msgChangeGroupEmployee"), employeeFIO, oldGroupName, newGroupName);
                }
            }

            if (leaveGroup && !joinCowork && !joinGroup)
            {
                // {0} �������� ������ ������ {1}, �� �� �������� ��� � �������� ����� ������. �� �������������, ��� {0} ������ �� ������� ������ ������ {1}?
                msg = String.Format(Resx.GetString("Users_msgRemoveEmployeeFromGroup"), employeeFIO, oldGroupName);
            }

            if (!leaveGroup && joinCowork)
            {
                // �� ���������� {0} �� ������� �����, �� ������� �������� �������� {1}. �� �������������, ��� {0} �������� �������� � {1}?
                msg = String.Format(Resx.GetString("Users_msgSetEmployeeOnShift"), employeeFIO, String.Join(" ,", fioList.ToArray()));
            }
            
            if (!leaveGroup && joinGroup)
            {
                // �� ���������� {0} �� ������� ����� ������ {1}. �� �������������, ��� {0} �������� ������ ������ {1}?
                msg = String.Format(Resx.GetString("Users_msgSetEmployeeOnGroup"), employeeFIO, newGroupName);
            }

            return msg;
        }

        /// <summary>
        /// ���������� ���������
        /// </summary>
        /// <param name="SaveWithCheck"></param>
		private void Save(bool SaveWithCheck = true)
		{
		    var msg = "";

            if (efWorkPlace.Value == "")
            {
                // ��� ���������� ���������� ������� ������� �����.
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
        /// �������� �������� �����
        /// </summary>
		private void DeleteAsc() {
            var msg = "";
		    var employeeFIO = "";

            var dt1 = DBManager.GetData(SQLQueries.SELECT_CHECK_����������������, Config.DS_user, CommandType.Text, 
                new Dictionary<string, object> { { "@�������������", Id } });
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                employeeFIO = dt1.Rows[0]["���"].ToString();

                var dt2 = DBManager.GetData(SQLQueries.SELECT_CHECK_������������������, Config.DS_user, CommandType.Text, 
                    new Dictionary<string, object> { { "@�������������", Id }, { "@���������������", efWorkPlace.Value } });
                if (dt2 != null && dt2.Rows.Count == 0)
                {
                    var dt3 = DBManager.GetData(SQLQueries.SELECT_FIO_����������������, Config.DS_user, CommandType.Text,
                        new Dictionary<string, object> { { "@�������������", Id } });

                    if (dt3 != null && dt3.Rows.Count > 0)
                    {
                        // {0} �������� ������ ������ {1}, �� �� �������� ��� � �������� ����� ������. �� �������������, ��� {0} ������ �� ������� ������ ������ {1}?
                        msg = String.Format(Resx.GetString("Users_msgRemoveEmployeeFromGroup"), employeeFIO, dt3.Rows[0]["���"]);
                    }
                }
            }
            
            msg += Resx.GetString("Users_msgConfirmDelete");
            ShowConfirm(msg, "cmdasync('cmd','Delete');", null);
		}

        /// <summary>
        /// �������� �������� ����� ����������
        /// </summary>
		private void Delete() {
            var sqlParams = new Dictionary<string, object>
            {
                { "@�������������", Id },
                { "@���������������",  string.IsNullOrEmpty(IdLoc) ? efWorkPlace.Value : IdLoc}
            };

            try
            {
                DBManager.ExecuteNonQuery(SQLQueries.DELETE_����������������������, CommandType.Text, Config.DS_user, sqlParams);
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