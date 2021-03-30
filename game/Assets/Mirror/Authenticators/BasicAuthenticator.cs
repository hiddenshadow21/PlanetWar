using System.Collections;
using UnityEngine;

namespace Mirror.Authenticators
{
    [AddComponentMenu("Network/Authenticators/BasicAuthenticator")]
    public class BasicAuthenticator : NetworkAuthenticator
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(BasicAuthenticator));

        [Header("Custom Properties")]

        // set these in the inspector
        public string username;
        public string password;

<<<<<<< HEAD
<<<<<<< HEAD
        #region Messages

=======
>>>>>>> feature/MenuEsc
=======
        #region Messages

>>>>>>> feature/MainMenu
        public struct AuthRequestMessage : NetworkMessage
        {
            // use whatever credentials make sense for your game
            // for example, you might want to pass the accessToken if using oauth
            public string authUsername;
            public string authPassword;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
            public byte code;
            public string message;
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
        #endregion

        #region Server

        /// <summary>
        /// Called on server from StartServer to initialize the Authenticator
        /// <para>Server message handlers should be registered in this method.</para>
        /// </summary>
<<<<<<< HEAD
=======
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
        public override void OnStartServer()
        {
            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
        /// <summary>
        /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
        /// </summary>
        /// <param name="conn">Connection to client.</param>
<<<<<<< HEAD
=======
        public override void OnStartClient()
        {
            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
        public override void OnServerAuthenticate(NetworkConnection conn)
        {
            // do nothing...wait for AuthRequestMessage from client
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
        /// <summary>
        /// Called on server when the client's AuthRequestMessage arrives
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        /// <param name="msg">The message payload</param>
<<<<<<< HEAD
=======
        public override void OnClientAuthenticate(NetworkConnection conn)
        {
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                authUsername = username,
                authPassword = password
            };

            conn.Send(authRequestMessage);
        }

>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
        public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Request: {0} {1}", msg.authUsername, msg.authPassword);

            // check the credentials by calling your web server, database table, playfab api, or any method appropriate.
            if (msg.authUsername == username && msg.authPassword == password)
            {
                // create and send msg to client so it knows to proceed
                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 100,
                    message = "Success"
                };

                conn.Send(authResponseMessage);

<<<<<<< HEAD
<<<<<<< HEAD
                // Accept the successful authentication
                ServerAccept(conn);
=======
                // Invoke the event to complete a successful authentication
                OnServerAuthenticated.Invoke(conn);
>>>>>>> feature/MenuEsc
=======
                // Accept the successful authentication
                ServerAccept(conn);
>>>>>>> feature/MainMenu
            }
            else
            {
                // create and send msg to client so it knows to disconnect
                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 200,
                    message = "Invalid Credentials"
                };

                conn.Send(authResponseMessage);

                // must set NetworkConnection isAuthenticated = false
                conn.isAuthenticated = false;

                // disconnect the client after 1 second so that response message gets delivered
                StartCoroutine(DelayedDisconnect(conn, 1));
            }
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
        IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            // Reject the unsuccessful authentication
            ServerReject(conn);
        }

        #endregion

        #region Client

        /// <summary>
        /// Called on client from StartClient to initialize the Authenticator
        /// <para>Client message handlers should be registered in this method.</para>
        /// </summary>
        public override void OnStartClient()
        {
            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

        /// <summary>
        /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
        /// </summary>
        /// <param name="conn">Connection of the client.</param>
        public override void OnClientAuthenticate(NetworkConnection conn)
        {
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                authUsername = username,
                authPassword = password
            };

            conn.Send(authRequestMessage);
        }

        /// <summary>
        /// Called on client when the server's AuthResponseMessage arrives
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        /// <param name="msg">The message payload</param>
<<<<<<< HEAD
=======
        public IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            conn.Disconnect();
        }

>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
        public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.message);

<<<<<<< HEAD
<<<<<<< HEAD
                // Authentication has been accepted
                ClientAccept(conn);
=======
                // Invoke the event to complete a successful authentication
                OnClientAuthenticated.Invoke(conn);
>>>>>>> feature/MenuEsc
=======
                // Authentication has been accepted
                ClientAccept(conn);
>>>>>>> feature/MainMenu
            }
            else
            {
                logger.LogFormat(LogType.Error, "Authentication Response: {0}", msg.message);

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
                // Authentication has been rejected
                ClientReject(conn);
            }
        }

        #endregion
<<<<<<< HEAD
=======
                // Set this on the client for local reference
                conn.isAuthenticated = false;

                // disconnect the client
                conn.Disconnect();
            }
        }
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
    }
}
