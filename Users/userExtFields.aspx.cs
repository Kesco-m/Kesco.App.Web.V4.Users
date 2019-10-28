using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;
using Convert = Kesco.Lib.ConvertExtention.Convert;
using Role = Kesco.Lib.BaseExtention.Enums.Corporate.Role;

namespace Kesco.App.Web.Users
{
    /// <summary>
    /// Класс страницы 
    /// </summary>
    public partial class UserExtFields : EntityPage
    {
        public string AreaRegistration = "&nbsp;";

        protected Employee employee;
        public string ReportServer = ConfigurationManager.AppSettings["URI_report"];
        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Инициализация контролов
        /// </summary>
        protected override void EntityFieldInit()
        {
            efStatus.DataItems = new Dictionary<string, object>
            {
                {"0", Resx.GetString("Users_fldWorks")}, // работает
                {"1", Resx.GetString("Users_fldDecreeHoliday")}, // декр.отпуск
                {"2", Resx.GetString("Users_fldGuestPersonal")}, // гость персональный
                {"3", Resx.GetString("Users_fldFired")}, // уволен
                {"4", Resx.GetString("Users_fldGuestCardGuest")}, // гость по гостевой карточке
                {"5", Resx.GetString("Users_fldOutsider")} // посторонний
            };

            efLang.DataItems = new Dictionary<string, object>
            {
                {"ru", "ru"},
                {"en", "en"},
                {"fr", "fr"},
                {"de", "de"}
            };

            efAreaRegistrationLink.Title = Resx.GetString("Users_lblOpenDossierOfIndividuals");
            efAreaRegistrationLink.Text =
                Resx.GetString("Users_ntfNoCountryOfRegistration"); // У физ.лица не указана страна регистрации

            efBirthDateLink.Title = Resx.GetString("Users_lblOpenDossierOfIndividuals");
            efBirthDateLink.Text = Resx.GetString("Users_ntfNoBirthDate"); // У физ.лица не указана дата рождения

            efINNLink.Title = Resx.GetString("Users_lblOpenDossierOfIndividuals"); // У физ.лица не указан ИНН
            efINNLink.Text = Resx.GetString("Users_ntfNoINN");

            btnUnLock.Visible = CurrentUser.HasRole((int)Role.Дежурный_инженер) || CurrentUser.HasRole((int)Role.Сотрудник_службы_поддержки_пользователей);

            base.EntityFieldInit();
        }


        protected override void EntityInitialization(Entity copy = null)
        {
            if (!V4IsPostBack)
            {
                if (string.IsNullOrEmpty(id) || id == "0") return;

                if (Entity == null)
                {
                    employee = new Employee(id);
                    Entity = employee;
                    efChanged.ChangedByID = null;
                }
                else
                {
                    employee = Entity as Employee;
                }
            }

            if (!id.IsNullEmptyOrZero()) OriginalEntity = new Employee(id);

            SetBinders();
        }

        private void SetBinders()
        {
            efRusLName.BinderValue = employee.LastNameBind.Value;
            efRusFName.BinderValue = employee.FirstNameBind.Value;
            efRusMName.BinderValue = employee.MiddleNameBind.Value;
            efEnLName.BinderValue = employee.LastNameEnBind.Value;
            efEnFName.BinderValue = employee.FirstNameEnBind.Value;
            efEnMName.BinderValue = employee.MiddleNameEnBind.Value;
            efStatus.BinderValue = employee.StatusBind.Value;
            efGuid.BinderValue = employee.GuidBind.Value;
            efAccountDisabled.BinderValue = employee.AccountDisabledBind.Value;
            efLogin.BinderValue = employee.LoginBind.Value;
            efDisplayName.BinderValue = employee.DisplayNameBind.Value;
            efEMail.BinderValue = employee.EmailBind.Value;
            efLang.BinderValue = employee.LanguageBind.Value;
            efPersonalFolder.BinderValue = employee.PersonalFolderBind.Value;
            efOrganization.BinderValue = employee.OrganizationIdBind.Value;
            efNotes.BinderValue = employee.NotesBind.Value;

            efRusLName.BindStringValue = employee.LastNameBind;
            efRusFName.BindStringValue = employee.FirstNameBind;
            efRusMName.BindStringValue = employee.MiddleNameBind;
            efEnLName.BindStringValue = employee.LastNameEnBind;
            efEnFName.BindStringValue = employee.FirstNameEnBind;
            efEnMName.BindStringValue = employee.MiddleNameEnBind;
            efStatus.BindStringValue = employee.StatusBind;
            efGuid.BindStringValue = employee.GuidBind;
            efAccountDisabled.BindStringValue = employee.AccountDisabledBind;
            efLogin.BindStringValue = employee.LoginBind;
            efDisplayName.BindStringValue = employee.DisplayNameBind;
            efEMail.BindStringValue = employee.EmailBind;
            efLang.BindStringValue = employee.LanguageBind;
            efPersonalFolder.BindStringValue = employee.PersonalFolderBind;
            efOrganization.BindStringValue = employee.OrganizationIdBind;
            efNotes.BindStringValue = employee.NotesBind;

            efRusLName.OriginalValue = ((Employee)OriginalEntity).LastName;
            efRusLName.OriginalValue = ((Employee)OriginalEntity).LastName;
            efRusFName.OriginalValue = ((Employee)OriginalEntity).FirstName;
            efRusMName.OriginalValue = ((Employee)OriginalEntity).MiddleName;
            efEnLName.OriginalValue = ((Employee)OriginalEntity).LastNameEn;
            efEnFName.OriginalValue = ((Employee)OriginalEntity).FirstNameEn;
            efEnMName.OriginalValue = ((Employee)OriginalEntity).MiddleNameEn;
            efStatus.OriginalValue = ((Employee)OriginalEntity).Status.ToString();
            efGuid.OriginalValue = ((Employee)OriginalEntity).Guid.ToString();
            efAccountDisabled.OriginalValue = ((Employee)OriginalEntity).AccountDisabled.ToString();
            efLogin.OriginalValue = ((Employee)OriginalEntity).Login;
            efDisplayName.OriginalValue = ((Employee)OriginalEntity).DisplayName;
            efEMail.OriginalValue = ((Employee)OriginalEntity).Email;
            efLang.OriginalValue = ((Employee)OriginalEntity).Language;
            efPersonalFolder.OriginalValue = ((Employee)OriginalEntity).PersonalFolder;
            efOrganization.OriginalValue = ((Employee)OriginalEntity).OrganizationId.ToString();
            efNotes.OriginalValue = ((Employee)OriginalEntity).Notes;
        }

        /// <summary>
        ///     Bind контролов
        /// </summary>
        private void BindFields()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", id } };
            var dt = DBManager.GetData(SQLQueries.SELECT_ADSI_ПоКодуСотрудника, Config.DS_user, CommandType.Text,
                sqlParams);
            HideCtrl("AccountExpiresPanel");
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["AccountExpires"] != null && dt.Rows[0]["AccountExpires"].ToString() != string.Empty)
                {
                    DisplayCtrl("AccountExpiresPanel");
                    efAccountExpires.Value = ((DateTime)dt.Rows[0]["AccountExpires"]).ToString("dd.MM.yyyy");
                }

                var path = dt.Rows[0]["Path"].ToString();
                var regex = new Regex("(OU=.+?(,|$))", RegexOptions.IgnoreCase);
                var matches = regex.Matches(path);
                var adsiPath = matches.Cast<object>().Aggregate("", (current, m) => current + m);
                if (adsiPath.Right(1) == ",") adsiPath = adsiPath.Left(adsiPath.Length - 1);
                efPath.Value = adsiPath;
            }

            CheckFields();

            efChanged.SetChangeDateTime = employee.Changed;
            efChanged.ChangedByID = employee.ChangedBy;
            efChanged.Flush();
        }

        /// <summary>
        ///     Формирует основной функционал страницы: подписи, меню, заголовок, title
        /// </summary>
        protected string RenderDocumentHeader()
        {
            using (var w = new StringWriter())
            {
                try
                {
                    if (ReturnId.IsNullEmptyOrZero())
                        SetMenuButtons();
                    RenderButtons(w);
                }
                catch (Exception e)
                {
                    var dex = new DetailedException(Resx.GetString("TTN_errFailedGenerateButtons") + ": " + e.Message,
                        e);
                    Logger.WriteEx(dex);
                    throw dex;
                }

                return w.ToString();
            }
        }

        /// <summary>
        ///     Инициализация/создание кнопок меню
        /// </summary>
        private void SetMenuButtons()
        {
            var btnEdit = MenuButtons.Find(btn => btn.ID == "btnEdit");
            RemoveMenuButton(btnEdit);

            var btnSave = MenuButtons.Find(btn => btn.ID == "btnSave");
            RemoveMenuButton(btnSave);

            var btnApply = MenuButtons.Find(btn => btn.ID == "btnApply");
            btnApply.Title = "Сохранить данные формы";

            var btnRefresh = MenuButtons.Find(btn => btn.ID == "btnRefresh");
            var btnReCheck = MenuButtons.Find(btn => btn.ID == "btnReCheck");

            var btnEquipment = new Button
            {
                ID = "btnEquipment",
                V4Page = this,
                Text = Resx.GetString("Users_lblEquipment"),
                Title = Resx.GetString("Users_cmdEquipmentDesription"),
                IconJQueryUI = ButtonIconsEnum.Wrench,
                OnClick = string.Format(
                    "var w = window.open('{0}&Id={1}&DT={2}', 'UserEquipment_{1}', 'menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes,width=800'); w.focus();",
                    ReportServer + "?/INVENTORY/Receipt&rc:parameters=false&rs:ClearSession=true",
                    id,
                    DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                )
            };
            AddMenuButton(btnEquipment);

            var btnPhoto = new Button
            {
                ID = "btnPhoto",
                V4Page = this,
                Text = Resx.GetString("Users_lblPhoto"),
                Title = Resx.GetString("Users_cmdPhotoDescription"),
                IconJQueryUI = ButtonIconsEnum.Person,
                OnClick = string.Format(
                    "var w = window.open('{0}','UserPhoto_{1}', 'menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes,width=500,height=600'); w.focus();",
                    Config.user_photos + "?Id=" + id, id)
            };
            AddMenuButton(btnPhoto);

            /*
            if (Request.QueryString["buh1s"] != null)
            {
                var btn1C = new Button
                {
                    ID = "btn1C",
                    V4Page = this,
                    Text = Resx.GetString("Users_MoveTo1C"),
                    Title = Resx.GetString("Users_MoveTo1C"),
                    IconJQueryUI = ButtonIconsEnum.Copy,
                    OnClick = "_sync();"
                };
                AddMenuButton(btn1C);
            }
            */
        }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "no-cache";

            IsRememberWindowProperties = true;
            WindowParameters = new WindowParameters("UserExtWndLeft", "UserExtWndTop", "UserExtWidth", "UserExtHeight");

            if (Entity != null) BindFields();

            // ToDo: Связь с 1С пока не реализована
            //if (Request.QueryString["buh1s"] != null)
            //{
            //    row1s.Visible = true;

            //    if (Request.QueryString["1s"] != null)
            //    {
            //        Kesco.Lib.Web.DataObjectModel.Env._1s.Sync_Справочник_Пользователи(int.Parse(Request.QueryString["buh1s"]), int.Parse(Request.QueryString["1s"]));
            //        Response.Redirect("http://ok.htm?id=" + Request.QueryString["1s"], true);
            //    }
            //}
            //Title = Kesco.Web.jScripts.ReadQueryParameter(Request, "title");
        }


        /// <summary>
        ///     Обработка клиентских команд
        /// </summary>
        /// <param name="cmd">Команды</param>
        /// <param name="param">Параметры</param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
            switch (cmd)
            {
                case "ShowItem":
                    ShowMessage(string.Format(
                            "Логин: ControlValue: <b>{0}</b>, EntityValue: <b>{1}</b> <br> Организация: ControlValue: <b>{2}</b>, EntityValue: <b>{3}</b>",
                            efLogin, employee.Login, efOrganization, employee.OrganizationId.ToString()), "Info",
                        MessageStatus.Information, "", 400, 100);
                    break;
                //case "Save":
                //    st = true;
                //    if (Entity.IsModified)
                //    {
                //        st = SaveData();
                //    }
                //    if (st) V4DropWindow();
                //    break;
                case "Apply":
                    if (Entity.IsModified)
                    {
                        SaveEntity();
                    }
                    break;
                case "ReCheck":
                    if (Entity.IsModified) CheckFields();
                    break;
                case "UnLock":
                    var sqlParams = new Dictionary<string, object>
                    {
                        {"@КодСотрудника", id}
                    };

                    try
                    {
                        DBManager.ExecuteNonQuery(SQLQueries.Сотрудник_Unlock, CommandType.Text, Config.DS_user, sqlParams);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message, "Error", MessageStatus.Error);
                    }

                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }

        /// <summary>
        ///     Проверка полей формы
        /// </summary>
        private void CheckFields()
        {
            var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", id } };
            var dt = DBManager.GetData(SQLQueries.SELECT_ДанныеСотрудникаИзЛиц, Config.DS_document, CommandType.Text,
                sqlParams);
            if (dt == null || dt.Rows.Count == 0)
            {
                DisplayCtrl("PersonInfoErrorPanel"); // Данные сотрудника не синхронизированы с физ. лицом
                HideCtrl("AreaRegistrationPanel");
                HideCtrl("BirthDatePanel");
                HideCtrl("INNPanel");
            }
            else
            {
                HideCtrl("PersonInfoErrorPanel");
                DisplayCtrl("AreaRegistrationPanel");
                DisplayCtrl("BirthDatePanel");
                if (dt.Rows[0]["ЕстьРеквизиты"].ToString().Equals("1"))
                {
                    JS.Write("$('#divErrNoRequzit').html('{0}');", "&nbsp;");
                    if (!dt.Rows[0]["Фамилия"].ToString().ToLower().Equals(employee.LastName.ToLower()))
                    {
                        DisplayCtrl("divErrFIO");
                        DisplayCtrl("divErrFIOEn");
                    }
                    else
                    {
                        HideCtrl("divErrFIO");
                        HideCtrl("divErrFIOEn");
                    }

                    if (!dt.Rows[0]["Имя"].ToString().ToLower().Equals(employee.FirstName.ToLower()))
                    {
                        DisplayCtrl("divErrFIO2");
                        DisplayCtrl("divErrFIOEn2");
                    }
                    else
                    {
                        HideCtrl("divErrFIO2");
                        HideCtrl("divErrFIOEn2");
                    }

                    if (!dt.Rows[0]["Отчество"].ToString().ToLower().Equals(employee.MiddleName.ToLower()))
                    {
                        DisplayCtrl("divErrFIO3");
                        DisplayCtrl("divErrFIOEn3");
                    }
                    else
                    {
                        HideCtrl("divErrFIO3");
                        HideCtrl("divErrFIOEn3");
                    }
                }
                else
                {
                    JS.Write("$('#divErrNoRequzit').html('{0}');", Resx.GetString("Users_msgNoValidDetails"));
                }

                if (dt.Rows[0]["Территория"].Equals(DBNull.Value))
                {
                    HideCtrl("efAreaRegistration");
                    DisplayCtrl("efAreaRegistrationLink");
                    efAreaRegistrationLink.OnClick = string.Format(
                        "var w = window.open('{0}?id={1}','user_{1}', 'menubar = no, location = no, resizable = yes, scrollbars = yes, status = yes'); w.focus();",
                        Config.person_form, dt.Rows[0]["КодЛица"]);
                }
                else
                {
                    HideCtrl("efAreaRegistrationLink");
                    DisplayCtrl("efAreaRegistration");
                    efAreaRegistration.Value = dt.Rows[0]["Территория"].ToString();
                    AreaRegistration = string.Format(Resx.GetString("Users_lblCountry") + " '{0}'",
                        dt.Rows[0]["Территория"]);
                }

                if (dt.Rows[0]["ДатаРождения"].Equals(DBNull.Value))
                {
                    HideCtrl("efBirthDate");
                    DisplayCtrl("efBirthDateLink");
                    efBirthDateLink.OnClick = string.Format(
                        "var w = window.open('{0}?id={1}','user_{1}', 'menubar = no, location = no, resizable = yes, scrollbars = yes, status = yes'); w.focus();",
                        Config.person_form, dt.Rows[0]["КодЛица"]);
                }
                else
                {
                    HideCtrl("efBirthDateLink");
                    DisplayCtrl("efBirthDate");
                    efBirthDate.Value = ((DateTime)dt.Rows[0]["ДатаРождения"]).ToString("dd.MM.yyyy");
                }

                if (dt.Rows[0]["Формат"].ToString().Equals("1"))
                {
                    DisplayCtrl("INNPanel");
                    if (dt.Rows[0]["ИНН"].ToString() == "")
                    {
                        HideCtrl("efINN");
                        DisplayCtrl("efINNLink");
                        efINNLink.OnClick = string.Format(
                            "var w = window.open('{0}?id={1}','user_{1}', 'menubar = no, location = no, resizable = yes, scrollbars = yes, status = yes'); w.focus();",
                            Config.person_form, dt.Rows[0]["КодЛица"]);
                    }
                    else
                    {
                        HideCtrl("efINNLink");
                        DisplayCtrl("efINN");
                        efINN.Value = dt.Rows[0]["ИНН"].ToString();
                    }
                }
                else
                {
                    HideCtrl("INNPanel");
                }
            }
        }

        /// <summary>
        ///     Кнопка: Сохранить
        /// </summary>
        private bool SaveData()
        {
            if (string.IsNullOrEmpty(id) || id == "0") return false;

            var sc = new StringCollection();
            Update(sc);

            if (sc.Count > 0)
            {
                var wi = WindowsIdentity.GetCurrent();
                var wp = new WindowsPrincipal(wi);

                if (wp.IsInRole(@"EURO\Domain Admins")  /*|| Page.User.IsInRole("TEST\\Programists")*/)
                {
                    if (Validation())
                    {
                        var sqlParams = new Dictionary<string, object> { { "@КодСотрудника", id } };
                        var sql = @"UPDATE Сотрудники SET " + Convert.Collection2Str(sc) +
                                  " WHERE КодСотрудника = @КодСотрудника";
                        DBManager.ExecuteNonQuery(sql, CommandType.Text, Config.DS_user, sqlParams);
                        return true;
                    }

                    return false;
                }

                if (CurrentUser.HasRole((int)Role.Кадровик) &&
                    efStatus.OriginalValue != employee.Status.ToString())
                {
                    if (sc.Count > 1)
                    {
                        ShowMessage("Вы можете менять только состояние сотрудника",
                            Resx.GetString("errDoisserWarrning"), MessageStatus.Error);
                        return false;
                    }

                    var sqlParams = new Dictionary<string, object>
                    {
                        {"@КодСотрудника", id},
                        {"@Состояние", efStatus.Value}
                    };

                    try
                    {
                        DBManager.ExecuteNonQuery(SQLQueries.UPDATE_СотрудникСостояние, CommandType.Text,
                            Config.DS_user, sqlParams);
                    }
                    catch (Exception e)
                    {
                        ShowMessage(e.Message, Resx.GetString("errDoisserWarrning"), MessageStatus.Error);
                        return false;
                    }
                }
                else
                {
                    ShowMessage(Resx.GetString("msgNoAccess"), Resx.GetString("errDoisserWarrning"),
                        MessageStatus.Error);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Валидация контролов
        /// </summary>
        /// <returns></returns>
        private bool Validation()
        {
            if (!efAccountDisabled.Checked)
            {
                //if (efLogin.Value.Trim() == "")
                //{
                //    // При указании состояния аккаунта необходимо указать логин.
                //    ShowMessage(Resx.GetString("Users_msgAccountStatusMustLogin."),
                //        Resx.GetString("errDoisserWarrning"), MessageStatus.Error);
                //    efLogin.Focus();
                //    return false;
                //}

                if (efRusLName.Value.Trim().Length > 0 && efEnLName.Value.Trim().Length == 0)
                {
                    // Не заполнено поле Last Name.
                    ShowMessage(Resx.GetString("Users_msgNoLastName"), Resx.GetString("errDoisserWarrning"),
                        MessageStatus.Error);
                    efEnLName.Focus();
                    return false;
                }

                if (efRusFName.Value.Trim().Length > 0 && efEnFName.Value.Trim().Length == 0)
                {
                    // Не заполнено поле First Name.
                    ShowMessage(Resx.GetString("Users_msgNoFirstName"), Resx.GetString("errDoisserWarrning"),
                        MessageStatus.Error);
                    efEnFName.Focus();
                    return false;
                }

                if (efRusMName.Value.Trim().Length > 0 && efEnMName.Value.Trim().Length == 0)
                {
                    // Не заполнено поле Middle Name.
                    ShowMessage(Resx.GetString("Users_msgNoMiddleName"), Resx.GetString("errDoisserWarrning"),
                        MessageStatus.Error);
                    efEnMName.Focus();
                    return false;
                }
            }

            return true;
        }


        protected override bool SaveEntity()
        {
            if (!SaveData()) return false;
            RefreshPage();
            return true;
        }

        /// <summary>
        ///     Подготовка скрипта обновления
        /// </summary>
        /// <param name="sc"></param>
        private void Update(StringCollection sc)
        {
            var oldEmployee = new Employee(id);

            if (efRusLName.Value != oldEmployee.LastName)
                sc.Add(string.Format("[Фамилия] = N'{0}'", efRusLName.Value));

            if (efRusFName.Value != oldEmployee.FirstName)
                sc.Add(string.Format("[Имя] = N'{0}'", efRusFName.Value));

            if (efRusMName.Value != employee.MiddleName)
                sc.Add(string.Format("[Отчество] = N'{0}'", efRusMName.Value));

            if (efEnLName.Value != oldEmployee.LastNameEn)
                sc.Add(string.Format("[LastName] = N'{0}'", efEnLName.Value));

            if (efEnFName.Value != oldEmployee.FirstNameEn)
                sc.Add(string.Format("[FirstName] = N'{0}'", efEnFName.Value));

            if (efEnMName.Value != oldEmployee.MiddleNameEn)
                sc.Add(string.Format("[MiddleName] = N'{0}'", efEnMName.Value));

            if (efOrganization.Value != oldEmployee.OrganizationId.ToString())
                sc.Add(string.Format("[КодЛицаЗаказчика] = '{0}'", efOrganization.Value));

            if (efStatus.Value != oldEmployee.Status.ToString())
                sc.Add(string.Format("[Состояние] = '{0}'", efStatus.Value));

            if (efLogin.Value != oldEmployee.Login)
                sc.Add(string.Format("[Login] = '{0}'", efLogin.Value));

            if (efAccountDisabled.Checked != (oldEmployee.AccountDisabled == 1))
                sc.Add(string.Format("[AccountDisabled] = {0}", efAccountDisabled.Checked ? "1" : "0"));

            if (efDisplayName.Value != oldEmployee.DisplayName)
                sc.Add(string.Format("[DisplayName] = '{0}'", efDisplayName.Value));

            if (efPersonalFolder.Value != oldEmployee.PersonalFolder)
                sc.Add(string.Format("[ЛичнаяПапка] = '{0}'", efPersonalFolder.Value));

            if (efLang.Value != oldEmployee.Language)
                sc.Add(string.Format("[Язык] = '{0}'", efLang.Value));

            if (efNotes.Value != oldEmployee.Notes)
                sc.Add(string.Format("[Примечания] = N'{0}'", efNotes.Value));
        }

        /// <summary>
        ///     Обработка события изменения логина
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void efLogin_OnChanged(object sender, ProperyChangedEventArgs e)
        {
            //if (e.NewValue == "")
            //{
            //    efAccountDisabled.Checked = true;
            //}
        }

        /// <summary>
        ///     Транслитерация
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string TransLit(string word)
        {
            var sqlParams = new Dictionary<string, object> { { "@word", word } };
            return DBManager.ExecuteScalar("SELECT dbo.fn_TransLit(@word)", CommandType.Text, Config.DS_user, sqlParams)
                .ToString();
        }

        /// <summary>
        ///     Событие изменения фамилии
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Аргументы</param>
        protected void efRusLName_OnChanged(object sender, ProperyChangedEventArgs e)
        {
            if (e.NewValue != "")
            {
                efEnLName.OnChanged(new ProperyChangedEventArgs(efEnLName.Value, TransLit(e.NewValue), efEnLName.OriginalValue));
                efEnLName.Value = TransLit(e.NewValue);
                efEnLName.Flush();
                CheckFields();
            }
        }

        /// <summary>
        ///     Событие изменения именни
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Аргументы</param>
        protected void efRusFName_OnChanged(object sender, ProperyChangedEventArgs e)
        {
            if (e.NewValue != "")
            {
                efEnFName.OnChanged(new ProperyChangedEventArgs(efEnFName.Value, TransLit(e.NewValue), efEnFName.OriginalValue));
                efEnFName.Value = TransLit(e.NewValue);
                efEnFName.Flush();
                CheckFields();
            }
        }

        /// <summary>
        ///     Событие изменения отчества
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Аргументы</param>
        protected void efRusMName_OnChanged(object sender, ProperyChangedEventArgs e)
        {
            if (e.NewValue != "")
            {
                efEnMName.OnChanged(new ProperyChangedEventArgs(efEnMName.Value, TransLit(e.NewValue), efEnMName.OriginalValue));
                efEnMName.Value = TransLit(e.NewValue);
                efEnMName.Flush();
                CheckFields();
            }
        }

        protected void efStatus_OnChanged(object sender, ProperyChangedEventArgs e)
        {
        }

        protected void efOrganization_OnChanged(object sender, ProperyChangedEventArgs e)
        {
        }

        protected void efLang_OnChanged(object sender, ProperyChangedEventArgs e)
        {
        }
    }
}