using DevourDev.Base.Security;
using DevourDev.MonoBase;
using System;
using System.Threading.Tasks;
using UnityEngine;
using FD.Global;
using FD.Networking.Gates.Packets;
using FD.ClientSide.Global;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

namespace FD.Networking.Client
{
    public class LogInHandler : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onConnectedToGarden;

        [SerializeField] private GameObject[] _logInInteface;
        [SerializeField] private GameObject[] _registrationInteface;

        [SerializeField] private TMPro.TMP_InputField _loginInput;
        [SerializeField] private TMPro.TMP_InputField _passwordInput;
        [SerializeField] private TMPro.TMP_InputField _passwordConfirmationInput;
        [SerializeField] private TMPro.TMP_InputField _emailInput;

        [SerializeField] private UnityEngine.UI.Button _logInButton;
        [SerializeField] private UnityEngine.UI.Button _registrateButton;
        [SerializeField] private UnityEngine.UI.Button _exitButton;

        [SerializeField] private TMPro.TextMeshProUGUI _sessionKeyText;

        private ClientManager _cm;


        private ClientManager CM
        {
            get
            {
                if (_cm == null)
                {
                    _cm = ClientManager.Instance;
                }
                return _cm;
            }
        }

        public async void FastQ1()
        {
           await SendLogInRequest("qweqwe1", "qweqwe");
        }
        public async void FastQ2()
        {
           await SendLogInRequest("qweqwe2", "qweqwe");
        }

        private async Task SendLogInRequest(string login, string pass)
        {
            LogInRequest req = new();
            req.Login = login;
            req.ClientSideHashedPassword = Hasher.HashString(pass);

            var response = await CM.RequestGates(req);

            if (response is LogInResponse lir)
            {
                if (lir.Result)
                {
                    goto SessionKeyReceived;
                }
                else
                {
                    _sessionKeyText.text = "Fail! " + lir.FailReason.ToString();
                    EnableButtons();
                    return;
                }
            }
            else
            {
                Debug.LogError("response is NOT LogInResponse");
                return;
            }

SessionKeyReceived:
            var connectedToGarden = await CM.TryConnectToGardenServerAsync(lir.SessionKey);
            if (connectedToGarden)
            {
                _onConnectedToGarden?.Invoke();
            }
            else
            {
                Debug.LogError("Unexpected fail.");
                //send bug report (message (?) to Gates Server)
                _loginInput.text = "unexpected fail";
                _passwordInput.text = "restart application";
                _passwordConfirmationInput.text = "and try again";
                StartCoroutine(AutoCloser());
            }
        }


        private void Awake()
        {
            _logInButton.onClick.AddListener(new UnityEngine.Events.UnityAction(LogInButtonHandler));
            _registrateButton.onClick.AddListener(new UnityEngine.Events.UnityAction(RegistrationButtonHandler));
            _exitButton.onClick.AddListener(() => Application.Quit(0));
        }



        public async void RegistrationButtonHandler()
        {
            if (_loginInput.text.Length > 20)
            {
                _loginInput.text = "max login length is 20";
                return;
            }

            if (_passwordInput.text.Length < 6)
            {
                _passwordInput.text = "min password length is 6";
                return;
            }

            if (_passwordInput.text != _passwordConfirmationInput.text)
            {
                _passwordInput.text = "passwords are...";
                _passwordConfirmationInput.text = "...not equal";
                return;
            }

            if (_emailInput.text.Length > 25)
            {
                _loginInput.text = "max email length is 25";
                return;
            }

            DisableButtons();
            SignUpRequest req = new();
            req.Login = _loginInput.text;
            req.ClientSideHashedPassword = Hasher.HashString(_passwordInput.text);
            req.ClientSideHashedPasswordConfirmation = Hasher.HashString(_passwordConfirmationInput.text);
            req.Email = _emailInput.text;

            var response = await CM.RequestGates(req);

            if (response is SignUpResponse sur)
            {
                if (sur.Result)
                {
                    goto SessionKeyReceived;

                    //System.Text.StringBuilder sb = new(sur.SessionKey.Length);
                    //foreach (var item in sur.SessionKey)
                    //{
                    //    sb.Append(item.ToString());
                    //}
                    //_sessionKeyText.text = sb.ToString();
                }
                else
                {
                    _sessionKeyText.text = "Fail! " + sur.FailReason.ToString();
                    EnableButtons();
                    return;
                }
            }
            else
            {
                Debug.Log("response is NOT SignUpResponse");
                return;
            }

           // EnableButtons();
//Debug.Log("Conencting to Garden...");
//var connectedSuccessfully = await _cm.TryConnectToGardenServerAsync(sur.SessionKey);
//Debug.Log("Conencting to Garden finished.");

//if (!connectedSuccessfully)
//{
//    Debug.LogError("Connecting to Garden failed.");
//    return;
//}


//Debug.Log("Connected to Garden!");

SessionKeyReceived:
            var connectedToGarden = await CM.TryConnectToGardenServerAsync(sur.SessionKey);
            if (connectedToGarden)
            {
                _onConnectedToGarden?.Invoke();
            }
            else
            {
                Debug.LogError("Unexpected fail.");
            }

        }

        private async void LogInButtonHandler()
        {
            if (_loginInput.text.Length > 20)
            {
                _loginInput.text = "max login length is 20.";
                return;
            }
            if (_passwordInput.text.Length < 6)
            {
                _passwordInput.text = "min password length is 6";
                return;
            }
            DisableButtons();
            LogInRequest req = new();
            req.Login = _loginInput.text;
            req.ClientSideHashedPassword = Hasher.HashString(_passwordInput.text);

            var response = await CM.RequestGates(req);

            if (response is LogInResponse lir)
            {
                if (lir.Result)
                {
                    goto SessionKeyReceived;

                    //System.Text.StringBuilder sb = new(lir.SessionKey.Length);
                    //foreach (var item in lir.SessionKey)
                    //{
                    //    sb.Append(item.ToString());
                    //}
                    //_sessionKeyText.text = sb.ToString();
                }
                else
                {
                    _sessionKeyText.text = "Fail! " + lir.FailReason.ToString();
                    EnableButtons();
                    return;
                }
            }
            else
            {
                Debug.LogError("response is NOT LogInResponse");
                return;
            }

            //var connectedSuccessfully = await _netHandler.TryConnectToGardenServerAsync(lir.SessionKey);

            //if (!connectedSuccessfully)
            //{
            //    Debug.LogError("Connecting to Garden failed.");
            //}

            //Debug.Log("Connected to Garden!");

            //EnableButtons();

SessionKeyReceived:
            var connectedToGarden = await CM.TryConnectToGardenServerAsync(lir.SessionKey);
            if (connectedToGarden)
            {
                _onConnectedToGarden?.Invoke();
            }
            else
            {
                Debug.LogError("Unexpected fail.");
                //send bug report (message (?) to Gates Server)
                _loginInput.text = "unexpected fail";
                _passwordInput.text = "restart application";
                _passwordConfirmationInput.text = "and try again";
                StartCoroutine(AutoCloser());
            }

        }

        private IEnumerator AutoCloser()
        {
            var w = new WaitForSecondsRealtime(1f);
            for (int i = 10; i > 0; i--)
            {
                _emailInput.text = $"autoclosing in {i}...";
                yield return w;
            }

            Application.Quit(228);
        }

        private void DisableButtons()
        {
            _logInButton.interactable = false;
            _registrateButton.interactable = false;
        }
        private void EnableButtons()
        {
            _logInButton.interactable = true;
            _registrateButton.interactable = true;
        }

    }
}