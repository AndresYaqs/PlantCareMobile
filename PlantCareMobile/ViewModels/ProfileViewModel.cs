using Firebase.Auth;
using PlantCareMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantCareMobile.ViewModels
{
    public class ProfileViewModel
    {
        #region "Properties"
        private string _userName; // <-- Nombre de usuario
        private string _email; // <-- Email de usuario
        private int _userPlantsCount; // <-- Cantidad de plantas del usuario
        private int _userRemindersCount; // <-- Cantidad de recordatorios del usuario
        private DateOnly memberSince; // <-- Fecha de registro del usuario
        private DateOnly lastLogin; // <-- Fecha del último login del usuario

        private readonly FirebaseAuthService _authService;
        private readonly ServerAPIService _serverapiService;

        public string UserName { get => _userName; set => _userName = value; }
        public string Email { get => _email; set => _email = value; }
        public int UserPlantsCount { get => _userPlantsCount; set => _userPlantsCount = value; }
        public int UserRemindersCount { get => _userRemindersCount; set => _userRemindersCount = value; }
        public DateOnly MemberSince { get => memberSince; set => memberSince = value; }
        public DateOnly LastLogin { get => lastLogin; set => lastLogin = value; }


        #endregion

        public ProfileViewModel(FirebaseAuthService authService, ServerAPIService serverAPIService) 
        {
            _authService = authService;
            _serverapiService = serverAPIService;

            LoadNecesaryData();
        }

        private void LoadNecesaryData()
        {
            LoadUsernameAndEmail();
            LoadUserInfoFromServer();
        }
        private void LoadUsernameAndEmail()
        {
            UserName = _authService.GetCurrentUserDisplayName();
            Email = _authService.GetCurrentUserEmail();
            if (!string.IsNullOrEmpty(UserName) && UserName.Contains("@"))
            {
                UserName = _authService.GetCurrentUserEmail();
                UserName = UserName.Split('@')[0];
            }
        }
        private void LoadUserInfoFromServer()
        {
            
        }
    }
}
