using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Networking.Database;
using DevourDev.Base.Generators;
using System.Linq;
using DevourDev.Base.Security;
using FD.Networking.Database.Entities.Account;
using DevourDev.Base;
using DevourDev.MonoExtentions;
using System.Threading.Tasks;
using DevourDev.MonoBase;

namespace FD.Visuals
{
    public class DatabaseVisualiser : MonoBehaviour
    {
        [SerializeField] DatabaseServerManager _databaseServer;

        [SerializeField] private string[] _accountsBackupPath = { "database", "accounts backup" };
        //@"database\accounts_backup\";

        [SerializeField] private TMPro.TextMeshProUGUI _accountsCountText;
        [SerializeField] private TMPro.TextMeshProUGUI _lastAccountLoginText;
        [SerializeField] private TMPro.TextMeshProUGUI _lastRequestText;
        [SerializeField] private TMPro.TextMeshProUGUI _lastActionResponseText;

        [SerializeField] private TMPro.TextMeshProUGUI _databaseLockingStateText;
        [SerializeField] private TMPro.TextMeshProUGUI _databaseLastLockingMethod;


        private StringGenerator _sg;
        private bool _genAccBtnDown;
        private float _genAccBtnDownDuration;

        private void Start()
        {
            _databaseServer.OnRequestReceived += RequestReceivedHandler;
            _databaseServer.OnNewAccountRegistrated += NewAccountRegistratedHandler;
            _sg = new StringGenerator(Character.EnglishAlphabet.Concat(Character.Numbers).ToArray()); // =)
            _genAccBtnDown = false;
            _genAccBtnDownDuration = 0;

            _databaseServer.Database.OnLockStatusChanged += Database_OnLockStatusChanged;

        }

        [ContextMenu(nameof(GenerateHundredThouthandsAccounts))]
        public async void GenerateHundredThouthandsAccounts()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 100_000; i++)
                {
                    GenerateAccount();
                }
            });

        }

        private void Database_OnLockStatusChanged(bool lockState, string methodName)
        {
            this.InvokeOnMainThread(() =>
            {
                _databaseLockingStateText.text = lockState ? "LOCKING" : "OPEN";
                _databaseLockingStateText.color = lockState ? Color.red : Color.green;

                _databaseLastLockingMethod.text = methodName;
            });

        }

        private void Update()
        {
            HandleGenerateAccButtonPressing();
        }

        private void HandleGenerateAccButtonPressing()
        {
            if (_genAccBtnDown)
            {
                if (_genAccBtnDownDuration >= 0.8f)
                {
                    GenerateAccount();
                }
                else
                {
                    _genAccBtnDownDuration += Time.deltaTime;
                }
            }
            else
            {
                _genAccBtnDownDuration = 0;
                return;
            }
        }

        private string GenerateValidEmail()
        {
            var sb = _sg.GenerateStringBuilder(6, 12);
            sb.Append('@');
            _sg.AppendRandomSymbolsToStringBuilder(sb, 2, 6);
            sb.Append('.');
            _sg.AppendRandomSymbolsToStringBuilder(sb, 2, 4);
            return sb.ToString();
        }

        public void HandleGenAccBtnPointerDown()
        {
            _genAccBtnDown = true;
        }

        public void HandleGenAccBtnPointerUp()
        {
            _genAccBtnDown = false;
        }

        public void GenerateAccount()
        {
            string validSourceLogin = _sg.GenerateString(6, 15);
            string sourcePass = _sg.GenerateString(6, 20);
            string pseudoClientSideHashedPass = Hasher.HashString(sourcePass);
            string serverSideHashedPass = Hasher.HashString(pseudoClientSideHashedPass);
            string validEmail;
            EmailAddress parsedEmail;
            do
            {
                validEmail = GenerateValidEmail();
            } while (!EmailAddress.TryParse(validEmail, out parsedEmail));

            if (_databaseServer.Database.TryRegistrateAccount(validSourceLogin, serverSideHashedPass, parsedEmail, out var acc))
            {
                NewAccountRegistratedHandler(acc);
            }
            else
            {
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    _lastActionResponseText.text = $"Account generation and registration attempt failed. " +
$"Generated login: {validSourceLogin}, " +
$"generated password: {sourcePass}, generated email: {validEmail}";
                }, nameof(GenerateAccount));

            }
        }

        public async void SaveAccounts()
        {
            var now = DateTime.Now;
            var fileName = now.ToShortDateString() + " " + now.ToShortTimeString();
            var validFileName = FileHandler.FixFileName(fileName);
            await Task.Run(() => _databaseServer.Database.SaveEntities(Path.Combine(_accountsBackupPath), validFileName));
        }

        public async void LoadAccounts()
        {
            var cts = new System.Threading.CancellationTokenSource();
            var token = cts.Token;
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _databaseServer.Database.ClearEntities();
                        var latestFilePath = DevourDev.Base.FileHandler.GetLatestWritenFileFileInDirectory(new DirectoryInfo(Path.Combine(_accountsBackupPath)));
                        _databaseServer.Database.LoadEntities(latestFilePath.FullName);
                        UnityMainThreadDispatcher.InvokeOnMainThread(() => { UpdateDatabaseElementsCount(); }, nameof(LoadAccounts));
                       
                    }
                    catch (NullReferenceException nre)
                    {
                        UnityMainThreadDispatcher.InvokeOnMainThread(() => { Debug.LogError("No files found: " + nre.Message); }, nameof(LoadAccounts));
                       
                        cts.Cancel();
                    }
                }, token);
            }
            catch (TaskCanceledException)
            {
                UnityMainThreadDispatcher.InvokeOnMainThread(() => { _lastActionResponseText.text = $"Ошибка при загрузке данных"; }, nameof(LoadAccounts));
                
                return;
            }
            catch (OperationCanceledException)
            {
                UnityMainThreadDispatcher.InvokeOnMainThread(() => { _lastActionResponseText.text = $"Ошибка при загрузке данных"; }, nameof(LoadAccounts));

                return;
            }

            _lastActionResponseText.text = $"Данные успешно загружены";

        }

        private void RequestReceivedHandler(Networking.IPacketContent req, Networking.ResponsingConnection connection)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                _lastRequestText.text = $"Последний запрос: {req.GetType()} от {connection.Connection.Socket.RemoteEndPoint}";
                UpdateDatabaseElementsCount();
            }, nameof(RequestReceivedHandler));

        }

        private void NewAccountRegistratedHandler(Networking.Database.Entities.Account.Account acc)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                _lastAccountLoginText.text = $"Логин последнего зарег. аккаунта: {acc.SecureData.LogInData.SourceLogin}";
                UpdateDatabaseElementsCount();
                _lastActionResponseText.text = $"Account generation and registration attempt succeed! " +
                   $"Generated login: {acc.SecureData.LogInData.SourceLogin}, " +
                   $"generated email: {acc.SecureData.LogInData.Email}";
            }, nameof(NewAccountRegistratedHandler));

        }

        private void UpdateDatabaseElementsCount()
        {
            _accountsCountText.text = $"Всего аккаунтов: {_databaseServer.Database.EntitiesCount}";
        }


    }
}
