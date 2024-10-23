namespace StellarRoles
{
    public class PlayerEndGameStats
    {
        //General
        public string Name { get; set; }
        public bool Disconnected { get; set; }
        public string Role { get; set; }

        //Crewmates
        public int CorrectVotes { get; set; }
        public int IncorrectVotes { get; set; }
        public int CorrectEjects { get; set; }
        public int IncorrectEjects { get; set; }
        public bool AliveAtLastMeeting { get; set; }
        public bool FirstTwoVictimsRound1 { get; set; }
        public int TasksTotal { get; set; }
        public int TasksCompleted { get; set; }
        public bool CriticalMeetingError { get; set; }

        //Imposter
        public int NumberOfCrewmatesEjectedTotal { get; set; }
        public int Kills { get; set; }
        public int Survivability { get; set; }
        public bool ImposterDisconnectedWhileAlive { get; set; }

        //Overall
        public string WinType { get; set; }
    }
}
