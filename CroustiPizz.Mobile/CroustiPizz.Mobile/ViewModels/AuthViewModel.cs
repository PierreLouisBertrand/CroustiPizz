using System.Threading.Tasks;
using System.Windows.Input;
using CroustiPizz.Mobile.Dtos;
using CroustiPizz.Mobile.Dtos.Accounts;
using CroustiPizz.Mobile.Dtos.Authentications;
using CroustiPizz.Mobile.Dtos.Authentications.Credentials;
using CroustiPizz.Mobile.Interfaces;
using CroustiPizz.Mobile.Pages;
using CroustiPizz.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CroustiPizz.Mobile.ViewModels
{
    /// <summary>
    /// Ce view model permet de gérer les informations affichées dans la vue d'authenfication.
    /// Il comprend également une gestion d'onglets à afficher (connexion ou inscription)
    /// </summary>
    public class AuthViewModel : ViewModelBase
    {
        /// <summary>
        /// Constante représentant la couleur du texte lorsque l'onglet est selectionné.
        /// </summary>
        public const string SELECTED_COLOR = "Black";
        /// 
        /// <summary>
        /// Constante représentant la couleur du texte lorsque l'onglet n'est pas sélectionné.
        /// </summary>
        public const string UNSELECTED_COLOR = "#BAC5DB";

        private string _loginTextColor;

        public string LoginTextColor
        {
            get => _loginTextColor;
            set => SetProperty(ref _loginTextColor, value);
        }

        private string _signupTextColor;

        public string SignUpTextColor
        {
            get => _signupTextColor;
            set => SetProperty(ref _signupTextColor, value);
        }

        private bool _showingLogin;

        public bool ShowingLogin
        {
            get => _showingLogin;
            set => SetProperty(ref _showingLogin, value);
        }

        private bool _showingSignup;

        public bool ShowingSignup
        {
            get => _showingSignup;
            set => SetProperty(ref _showingSignup, value);
        }

        private string _email;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _mdp;

        public string Mdp
        {
            get => _mdp;
            set => SetProperty(ref _mdp, value);
        }

        private string _nom;

        public string Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }

        private string _prenom;

        public string Prenom
        {
            get => _prenom;
            set => SetProperty(ref _prenom, value);
        }

        private string _emailRegister;

        public string EmailRegister
        {
            get => _emailRegister;
            set => SetProperty(ref _emailRegister, value);
        }

        private string _mdp1;

        public string Mdp1
        {
            get => _mdp1;
            set => SetProperty(ref _mdp1, value);
        }

        private string mdp2;

        public string Mdp2
        {
            get => mdp2;
            set => SetProperty(ref mdp2, value);
        }

        private string _numero;

        public string Numero
        {
            get => _numero;
            set => SetProperty(ref _numero, value);
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isSubmitEnabled = true;

        public bool IsSubmitEnabled
        {
            get => _isSubmitEnabled;
            set => SetProperty(ref _isSubmitEnabled, value);
        }

        public ICommand SignUpCommand { get; }
        public ICommand LoginCommand { get; }

        public ICommand ShowSignUpCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public IUserApiService UserApiService { get; }

        public AuthViewModel()
        {
            UserApiService = DependencyService.Get<IUserApiService>();

            ShowSignUpCommand = new Command(ShowSignUpAction);
            ShowLoginCommand = new Command(ShowLoginAction);

            SignUpCommand = new Command(SignUpAction);
            LoginCommand = new Command(LoginAction);

            ShowingLogin = true;
            ShowingSignup = !ShowingLogin;

            LoginTextColor = SELECTED_COLOR;
            SignUpTextColor = UNSELECTED_COLOR;
        }

        private async void SignUpAction()
        {
            if (Mdp1 == Mdp2)
            {
                CreateUserRequest utilisateur = new CreateUserRequest
                {
                    Email = EmailRegister,
                    FirstName = Prenom,
                    LastName = Nom,
                    PhoneNumber = Numero,
                    Password = Mdp1,
                    ClientId = Constantes.CLIENT_ID,
                    ClientSecret = Constantes.CLIENT_SECRET
                };

                IsLoading = true;
                IsSubmitEnabled = false;

                Response<LoginResponse> response = await UserApiService.RegisterUser(utilisateur);

                if (response.IsSuccess)
                {
                    AllerPageAccueil();
                    ResetActivityIndicatorAndSubmitButton();
                }
                else
                {
                    ResetActivityIndicatorAndSubmitButton();
                    DependencyService.Get<IMessage>()
                        .LongAlert(Resources.AppResources.AlertFormError + response.ErrorMessage);
                }
            }
            else
            {
                ResetActivityIndicatorAndSubmitButton();
                DependencyService.Get<IMessage>().LongAlert(Resources.AppResources.AlertPasswordMismatch);
            }
        }

        private async void LoginAction()
        {
            LoginWithCredentialsRequest utilisateur = new LoginWithCredentialsRequest()
            {
                Login = Email,
                Password = Mdp,
                ClientId = Constantes.CLIENT_ID,
                ClientSecret = Constantes.CLIENT_SECRET
            };

            IsLoading = true;
            IsSubmitEnabled = false;

            Response<LoginResponse> response = await UserApiService.LoginUser(utilisateur);

            if (response.IsSuccess)
            {
                AllerPageAccueil();
                ResetActivityIndicatorAndSubmitButton();
            }
            else
            {
                ResetActivityIndicatorAndSubmitButton();
                DependencyService.Get<IMessage>().LongAlert(Resources.AppResources.AlertLoginOrPasswordError);
            }
        }

        private void ResetActivityIndicatorAndSubmitButton()
        {
            IsLoading = false;
            IsSubmitEnabled = true;
        }

        private void ShowSignUpAction()
        {
            ShowingLogin = false;
            ShowingSignup = !ShowingLogin;

            LoginTextColor = UNSELECTED_COLOR;
            SignUpTextColor = SELECTED_COLOR;
        }

        private void ShowLoginAction()
        {
            ShowingLogin = true;
            ShowingSignup = !ShowingLogin;

            LoginTextColor = SELECTED_COLOR;
            SignUpTextColor = UNSELECTED_COLOR;
        }

        private void AllerPageAccueil()
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync<MainView>();
        }

        public override async Task OnResume()
        {
            string accessToken = await SecureStorage.GetAsync(Constantes.ACCESS_TOKEN);
            string refreshToken = await SecureStorage.GetAsync(Constantes.REFRESH_TOKEN);
            if (accessToken != null && refreshToken != null)
            {
                AllerPageAccueil();
            }

            await base.OnResume();
        }
    }
}