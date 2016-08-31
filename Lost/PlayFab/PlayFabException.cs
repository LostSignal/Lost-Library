
namespace Lost
{
    using System;
    using PlayFab;

    public class PlayFabException : Exception
    {
        public PlayFabException(PlayFabError error)
        {
            this.Error = error;
        }

        public PlayFabError Error { get; private set; }
    }
}
