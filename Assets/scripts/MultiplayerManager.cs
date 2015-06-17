using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

public class MultiplayerManager : MonoBehaviour
{
	private const int Opponents = 1;
	private const int Variant = 0;

	TurnBasedMatch mIncomingMatch = null;
	Invitation mIncomingInvite = null;

	bool mInMatch = false;
	System.Action<bool> mAuthCallback;


	void Start()
	{
		/*
		mAuthCallback = (bool success) => {
			if (success && !mInMatch) {
				SwitchToMain();
			}
		};
		*/
		
		PlayGamesClientConfiguration config = 
			new PlayGamesClientConfiguration.Builder()
				.WithInvitationDelegate(OnGotInvitation)
				.WithMatchDelegate(OnGotMatch)
				.Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate();
		PlayGamesPlatform.DebugLogEnabled = true;
		
		PlayGamesPlatform.Instance.Authenticate(OnAuth, true);
	}

	void OnAuth(bool success)
	{
		if (success)
		{
			// TODO: Show UI
		}
	}

	#region User Actions

	public void OnQuickMatch()
	{
		PlayGamesPlatform.Instance.TurnBased.CreateQuickMatch(Opponents, Opponents, Variant, OnMatchStarted);
	}
	
	public void OnInvite()
	{
		PlayGamesPlatform.Instance.TurnBased.CreateWithInvitationScreen(Opponents, Opponents, Variant, OnMatchStarted);
	}
	
	public void OnInbox()
	{
		PlayGamesPlatform.Instance.TurnBased.AcceptFromInbox(OnMatchStarted);
	}

	public void OnAcceptInvitation()
	{
		PlayGamesPlatform.Instance.TurnBased.AcceptInvitation(mIncomingInvite.InvitationId, OnMatchStarted);
	}

	public void OnDeclineInvitation()
	{
		PlayGamesPlatform.Instance.TurnBased.DeclineInvitation(mIncomingInvite.InvitationId);
	}
	
	public void OnSignOut()
	{
		PlayGamesPlatform.Instance.SignOut ();
	}

	#endregion

	protected void OnGotInvitation(Invitation invitation, bool shouldAutoAccept)
	{
		if (invitation.InvitationType != Invitation.InvType.TurnBased) {
			// wrong type of invitation!
			return;
		}
		
		if (shouldAutoAccept)
		{
			PlayGamesPlatform.Instance.TurnBased.AcceptInvitation(invitation.InvitationId, OnMatchStarted);
		} else {
			mIncomingInvite = invitation;
		}
	}


	protected void OnGotMatch(TurnBasedMatch match, bool shouldAutoLaunch) {
		if (shouldAutoLaunch) {
			OnMatchStarted(true, match);
		} else {
			mIncomingMatch = match;
		}
	}


	public void OnMatchStarted(TurnBasedMatch match) {
		mMatch = match;

		if (mMatch == null) {
			throw new System.Exception("PlayGui can't be started without a match!");
		}
		try {
			// Note that mMatch.Data might be null (when we are starting a new match).
			// MatchData.MatchData() correctly deals with that and initializes a
			// brand-new match in that case.
			mMatchData = new MatchData(mMatch.Data);
		} catch (MatchData.UnsupportedMatchFormatException ex) {
			mFinalMessage = "Your game is out of date. Please update your game\n" +
				"in order to play this match.";
			Debug.LogWarning("Failed to parse board data: " + ex.Message);
			return;
		}
		
		// determine if I'm the 'X' or the 'O' player
		mMyMark = mMatchData.GetMyMark(match.SelfParticipantId);
		
		bool canPlay = (mMatch.Status == TurnBasedMatch.MatchStatus.Active &&
		                mMatch.TurnStatus == TurnBasedMatch.MatchTurnStatus.MyTurn);
		
		if (canPlay) {
			mShowInstructions = true;
		} else {
			mFinalMessage = ExplainWhyICantPlay();
		}
		
		// if the match is in the completed state, acknowledge it
		if (mMatch.Status == TurnBasedMatch.MatchStatus.Complete) {
			PlayGamesPlatform.Instance.TurnBased.AcknowledgeFinished(mMatch,
			                                                         (bool success) => {
				if (!success) {
					Debug.LogError("Error acknowledging match finish.");
				}
			});
		}
		
		// set up the objects to show the match to the player
		SetupObjects(canPlay);
	}
}
