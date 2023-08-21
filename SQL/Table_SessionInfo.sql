Create table SessionInfo(
    SessionID integer primary key autoincrement,
    SessionStartTime datetime,
    SessionEndTime datetime,
    TotalSessionDuration datetime,
    TrackedDirectory text,
    TotalActiveTime datetime,
    TotalAFKTime datetime,
    TotalTimesAFK integer,
    TotalEvents integer,
    TotalCreations integer,
    TotalDeletions integer,
    TotalRenamings integer,
    TotalErrors integer,
    SessionWasClosedBySystemEvent bool,
    DefaultAFKStartLimitInMiliseconds long);