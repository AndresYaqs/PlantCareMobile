using Firebase.Auth;
using Firebase.Auth.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantCareMobile.Services
{
    public class FirebaseAuthService
    {
        private const string _firebaseApiKey = ""; //API PUBLICA DE FIREBASE AQUÍ

        private readonly FirebaseAuthClient _authClient;
        private UserCredential _userCredential;

        public static string FirebaseApiKey => _firebaseApiKey;

        public FirebaseAuthService()
        {
            // Configurar FirebaseAuthClient
            var config = new FirebaseAuthConfig
            {
                ApiKey = FirebaseApiKey,
                AuthDomain = "plantcare-5bdfb.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider(),
                }
            };

            _authClient = new FirebaseAuthClient(config);
        }
        // Registro de nuevo usuario
        public async Task<(bool Success, string Message, string Token)> RegisterAsync(string email, string password)
        {
            try
            {
                _userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

                var token = await _userCredential.User.GetIdTokenAsync();

                return (true, "Usuario registrado exitosamente", token);
            }
            catch (FirebaseAuthException ex)
            {
                return (false, GetErrorMessage(ex), null);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        // Login de usuario existente
        public async Task<(bool Success, string Message, string Token, string UserId)> LoginAsync(string email, string password)
        {
            try
            {
                _userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                var token = await _userCredential.User.GetIdTokenAsync();
                var userId = _userCredential.User.Uid;

                return (true, "Login exitoso", token, userId);
            }
            catch (FirebaseAuthException ex)
            {
                return (false, GetErrorMessage(ex), null, null);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null, null);
            }
        }

        // Obtener token actual (para requests al backend)
        public async Task<string> GetCurrentTokenAsync()
        {
            if (_userCredential?.User == null)
            {
                return null;
            }

            try
            {
                // Firebase tokens expiran cada 1 hora, este método obtiene uno válido
                return await _userCredential.User.GetIdTokenAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Verificar si hay usuario autenticado
        public bool IsAuthenticated()
        {
            return _userCredential?.User != null;
        }

        // Obtener información del usuario actual
        public string GetCurrentUserId()
        {
            return _userCredential?.User?.Uid;
        }

        public string GetCurrentUserEmail()
        {
            return _userCredential?.User?.Info?.Email;
        }

        // Cerrar sesión
        public void Logout()
        {
            _authClient.SignOut();
            _userCredential = null;
        }

        // Recuperar contraseña
        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email)
        {
            try
            {
                await _authClient.ResetEmailPasswordAsync(email);
                return (true, "Correo de recuperación enviado");
            }
            catch (FirebaseAuthException ex)
            {
                return (false, GetErrorMessage(ex));
            }
        }

        // Mensajes de error amigables
        private string GetErrorMessage(FirebaseAuthException ex)
        {
            return ex.Reason switch
            {
                AuthErrorReason.EmailExists => "Este correo ya está registrado",
                AuthErrorReason.InvalidEmailAddress => "Correo electrónico inválido",
                AuthErrorReason.WeakPassword => "La contraseña debe tener al menos 6 caracteres",
                AuthErrorReason.WrongPassword => "Contraseña incorrecta",
                AuthErrorReason.UserNotFound => "Usuario no encontrado",
                AuthErrorReason.TooManyAttemptsTryLater => "Demasiados intentos. Intenta más tarde",
                _ => $"Error de autenticación: {ex.Message}"
            };
        }
    }
}
