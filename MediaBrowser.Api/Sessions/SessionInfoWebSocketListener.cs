using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Net;
using MediaBrowser.Controller.Session;
using Microsoft.Extensions.Logging;

namespace MediaBrowser.Api.Sessions
{
    /// <summary>
    /// Class SessionInfoWebSocketListener
    /// </summary>
    public class SessionInfoWebSocketListener : BasePeriodicWebSocketListener<IEnumerable<SessionInfo>, WebSocketListenerState>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        protected override string Name => "Sessions";

        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly ISessionManager _sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInfoWebSocketListener"/> class.
        /// </summary>
        public SessionInfoWebSocketListener(ILogger<SessionInfoWebSocketListener> logger, ISessionManager sessionManager)
            : base(logger)
        {
            _sessionManager = sessionManager;

            _sessionManager.SessionStarted += OnSessionManagerSessionStarted;
            _sessionManager.SessionEnded += OnSessionManagerSessionEnded;
            _sessionManager.PlaybackStart += OnSessionManagerPlaybackStart;
            _sessionManager.PlaybackStopped += OnSessionManagerPlaybackStopped;
            _sessionManager.PlaybackProgress += OnSessionManagerPlaybackProgress;
            _sessionManager.CapabilitiesChanged += OnSessionManagerCapabilitiesChanged;
            _sessionManager.SessionActivity += OnSessionManagerSessionActivity;
        }

        private void OnSessionManagerSessionActivity(object sender, SessionEventArgs e)
        {
            SendData(false);
        }

        private void OnSessionManagerCapabilitiesChanged(object sender, SessionEventArgs e)
        {
            SendData(true);
        }

        private void OnSessionManagerPlaybackProgress(object sender, PlaybackProgressEventArgs e)
        {
            SendData(!e.IsAutomated);
        }

        private void OnSessionManagerPlaybackStopped(object sender, PlaybackStopEventArgs e)
        {
            SendData(true);
        }

        private void OnSessionManagerPlaybackStart(object sender, PlaybackProgressEventArgs e)
        {
            SendData(true);
        }

        private void OnSessionManagerSessionEnded(object sender, SessionEventArgs e)
        {
            SendData(true);
        }

        private void OnSessionManagerSessionStarted(object sender, SessionEventArgs e)
        {
            SendData(true);
        }

        /// <summary>
        /// Gets the data to send.
        /// </summary>
        /// <returns>Task{SystemInfo}.</returns>
        protected override Task<IEnumerable<SessionInfo>> GetDataToSend()
        {
            return Task.FromResult(_sessionManager.Sessions);
        }

        /// <inheritdoc />
        protected override void Dispose(bool dispose)
        {
            _sessionManager.SessionStarted -= OnSessionManagerSessionStarted;
            _sessionManager.SessionEnded -= OnSessionManagerSessionEnded;
            _sessionManager.PlaybackStart -= OnSessionManagerPlaybackStart;
            _sessionManager.PlaybackStopped -= OnSessionManagerPlaybackStopped;
            _sessionManager.PlaybackProgress -= OnSessionManagerPlaybackProgress;
            _sessionManager.CapabilitiesChanged -= OnSessionManagerCapabilitiesChanged;
            _sessionManager.SessionActivity -= OnSessionManagerSessionActivity;

            base.Dispose(dispose);
        }
    }
}
