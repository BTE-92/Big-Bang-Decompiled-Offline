public static class ServerConfig
{
	public const string HPW = "bfid3Z53SFib325PJGFasae";

	public const string CONTENT_JSON = "application/json";

	public const string CONTENT_BYTE = "application/octet-stream";

	public const string CONTENT_GHOST = "application/ghost";

	public const string NETWORK_FACEBOOK = "facebook";

	public const string NETWORK_GAMECENTER = "gamecenter";

	public const string NETWORK_GOOGLE = "google";

	public const string PLAYER_CREATE = "/v4/player/login";

	public const string PLAYER_STATS_CHANGE = "/v1/player/stats/change";

	public const string GAME_CENTER_SWITCH = "/v2/player/join/gameCenter";

	public const string GAME_CENTER_CHECK = "/v2/player/check/gameCenter";

	public const string GAME_CENTER_FRIENDS = "/v1/player/follow/gameCenter";

	public const string FACEBOOK_JOIN = "/v2/player/join/facebook";

	public const string NINJA_JOIN = "/v1/player/join/ninja";

	public const string FOLLOW_PLAYER = "/v1/player/follow/add";

	public const string UNFOLLOW_PLAYER = "/v1/player/follow/remove";

	public const string FOLLOWEES = "/v1/player/follow/followees";

	public const string FOLLOWERS = "/v1/player/follow/followers";

	public const string FRIENDS = "/v1/player/friends";

	public const string SET_NUMBER = "/v1/player/data/setNumber";

	public const string SET_STRING = "/v1/player/data/setString";

	public const string FIND_PLAYER_DATA = "/v1/player/data/find";

	public const string REMOVE_PLAYER_DATA = "/v1/player/data/remove";

	public const string PLAYER_SET_MAX_TESTERS = "/v1/player/tester/setMax";

	public const string USE_TESTER = "/v1/player/tester/use";

	public const string RESET_LEVEL = "/v1/player/resetLevel";

	public const string PURCHASE_TESTER = "/v1/player/tester/purchase";

	public const string PLAYER_STATS_SET = "/v1/player/stats/set";

	public const string PLAYER_DATA_SET_V2 = "/v2/player/data/change";

	public const string PLAYER_VEHICLE_RENT = "/v1/player/rent";

	public const string SKIP_LEVEL = "/v1/player/skip";

	public const string PLAYER_NAME_CHANGE = "/v1/player/changeName";

	public const string REMOVE_PLAYER = "/v1/player/remove";

	public const string PLAYER_SKIP_TIMER = "/v1/player/skipTimeLeft";

	public const string RESET_SKIP_TIMER = "/v1/player/resetSkipTimer";

	public const string GET_PLAYER_INFO = "/v1/player/info";

	public const string GET_PLAYER_STATS = "/v1/player/stats/find";

	public const string CLAIM_TUTORIAL = "/v1/tutorial/claim";

	public const string SOCIAL_CHECK = "/v1/player/check/social";

	public const string SOCIAL_SOLVE = "/v1/player/solve/social";

	public const string SOCIAL_FOLLOW = "/v1/player/follow/social";

	public const string SOCIAL_REMOVE = "/v1/player/remove/social";

	public const string ADD_CUSTOMISATION = "/v1/customisation/add";

	public const string UPSERT_ACHIEVEMENT = "/v1/achievement/upsert";

	public const string MINIGAME_GET = "/v1/minigame/meta/find";

	public const string MINIGAME_OWN_PUBLISHED_FIND = "/v1/minigame/own/published";

	public const string MINIGAME_OWN_FIND = "/v1/minigame/own";

	public const string MINIGAME_OWN_SAVED_FIND = "/v1/minigame/own/saved";

	public const string MINIGAME_PUBLISHED_FIND = "/v1/minigame/meta/published";

	public const string MINIGAME_FIND_WITH_ITEMS = "/v1/minigame/meta/findItems";

	public const string MINIGAME_DATA_FIND = "/v1/minigame/data/find";

	public const string MINIGAME_DATA_SAVE = "/v1/minigame/data/save";

	public const string MINIGAME_SAVE = "/v2/minigame/save";

	public const string MINIGAME_PUBLISH = "/v3/minigame/publish";

	public const string MINIGAME_HIDE = "/v1/minigame/hide";

	public const string MINIGAME_LIKE = "/v1/minigame/like/save";

	public const string MINIGAME_DELETE = "/v1/minigame/delete";

	public const string MINIGAME_TREND_FIND = "/v1/minigame/trend/find";

	public const string MINIGAME_POPULAR_FIND = "/v1/minigame/popular/find";

	public const string MINIGAME_UNRATED_FIND = "/v1/minigame/unrated/find";

	public const string MINIGAME_ONE_FRESH = "/v1/minigame/oneFresh";

	public const string MINIGAME_FOLLOWEE_FIND = "/v1/minigame/followee/find";

	public const string MINIGAME_LEVEL_UPDATE = "/v1/minigame/level/update";

	public const string MINIGAME_REWARD_CLAIM = "/v1/minigame/claim";

	public const string MINIGAME_FOLLOWEE_PUBLISHED_FIND = "/v1/minigame/followee/published";

	public const string MINIGAME_HISTORY_FIND = "/v1/minigame/history";

	public const string MINIGAME_SUBGENRE_FIND = "/v1/minigame/subgenre/find";

	public const string MINIGAME_SEARCH = "/v2/minigame/meta/search";

	public const string MINIGAME_HIDDEN_FIND = "/v1/minigame/hidden";

	public const string MINIGAME_REWARDS_GET = "/v1/minigame/getRewards";

	public const string MINIGAME_REWARDS_CLAIM = "/v1/minigame/claimAll";

	public const string TEMPLATE_SAVE = "/v1/template/save";

	public const string TEMPLATE_LIST = "/v1/template/list";

	public const string TEMPLATE_FIND = "/v1/template/find";

	public const string TEMPLATE_DUPLICATE = "/v1/template/duplicate";

	public const string TOURNAMENT_PARTICIPATE = "/v1/tournament/unlock";

	public const string TOURNAMENTS_GET = "/v1/tournament/find";

	public const string TOURNAMENT_GET = "/v1/tournament/get";

	public const string TOURNAMENT_FIND = "/v1/tournament/find";

	public const string TOURNAMENT_CLAIM = "/v1/tournament/claim";

	public const string TOURNAMENT_CREATE = "/v1/tournament/create";

	public const string TOURNAMENT_SCORE_SEND = "/v2/tournament/score/send";

	public const string TOURNAMENT_SCORE_NEXT = "/v1/tournament/score/next";

	public const string TOURNAMENT_SCORE_FIND = "/v2/tournament/score/find";

	public const string CHALLENGE_GET_DAILY = "/v1/challenge/daily/find";

	public const string VERSUS_CREATE = "/v4/versus/create";

	public const string VERSUS_SCORE_OVERWRITE = "/v2/versus/score/overwrite";

	public const string VERSUS_QUIT = "/v1/versus/score/quit";

	public const string VERSUS_WIN = "/v1/versus/win";

	public const string VERSUS_FORFEIT = "/v1/versus/forfeit";

	public const string VERSUS_SET_TRIES = "/v1/versus/tries/set";

	public const string VERSUS_CLAIM = "/v1/versus/claim";

	public const string VERSUS_CLAIM_SAVE = "/v1/versus/claimAndSave";

	public const string FRIENDLY_CREATE = "/v1/friendly/create";

	public const string FRIENDLY_JOIN = "/v1/friendly/join";

	public const string FRIENDLY_DECLINE = "/v1/friendly/decline";

	public const string FRIENDLY_SCORE_OVERWRITE = "/v1/friendly/score/overwrite";

	public const string FRIENDLY_QUIT = "/v1/friendly/score/quit";

	public const string FRIENDLY_WIN = "/v1/friendly/win";

	public const string FRIENDLY_FORFEIT = "/v1/friendly/forfeit";

	public const string FRIENDLY_SET_TRIES = "/v1/friendly/tries/set";

	public const string FRIENDLY_CLAIM = "/v1/friendly/claim";

	public const string FRIENDLY_CLAIM_SAVE = "/v1/friendly/claimAndSave";

	public const string SCREENSHOT_SAVE = "/v1/minigame/screenshot/save";

	public const string SCREENSHOT_FIND = "/v1/minigame/screenshot/find";

	public const string RATING_SAVE = "/v1/rating/save";

	public const string GHOST_NEXT = "/v1/ghost/next";

	public const string GHOST_NEXT_V2 = "/v2/ghost/next";

	public const string GHOST_NEXT_V3 = "v1/ghosts";

	public const string GHOST_CREATOR = "/v1/ghost/creator/get";

	public const string GHOST_VERSUS = "/v1/ghost/versus/get";

	public const string GHOST_FRIENDLY = "/v1/ghost/friendly/get";

	public const string HIGHSCORE_SEND = "/v5/highscore/send";

	public const string HIGHSCORE_FIND = "/v2/highscore/find";

	public const string HIGHSCORE_QUIT = "/v2/highscore/quit";

	public const string HIGHSCORE_NEXT = "/v1/highscore/next";

	public const string COMMENT_SAVE = "/v1/minigame/comment/save";

	public const string COMMENT_FIND = "/v1/minigame/comment/find";

	public const string PUSH_TOKEN_SEND = "/v1/push/token/save";

	public const string PUSH_TOKEN_REPLACE = "/v1/push/token/replace";

	public const string PUSH_TOKEN_REMOVE = "/v1/push/token/remove";

	public const string NOTIFICATIONS_FIND = "/v1/notification/find";

	public const string NOTIFICATIONS_COUNT = "/v1/notification/count";

	public const string PLANET_META_FIND = "/v1/planet/current/find";

	public const string REWARD_CLAIM = "/v1/reward/claim";

	public const string RESEARCH_START = "/v1/research/start";

	public const string RESEARCH_CANCEL = "/v1/research/cancel";

	public const string RESEARCH_PURCHASE = "/v1/research/purchase";

	public const string RESEARCH_CLAIM = "/v1/research/claim";

	public const string TIMED_EVENT_GET = "/v1/metagame/getTimed";

	public const string PATH_DB_FIND = "/v1/path/db/find";

	public const string PATH_DB_UPDATE = "/v2/path/db/update";

	public const string PATH_RESET = "/v1/path/reset";

	public const string PATH_PROGRESSION_GET = "/v1/path/get";

	public const string ADNETWORK_CONFIG_GET = "/v1/ads/config";

	public const string ANALYTICS_TRACK_PURCHASE = "/v1/analytics/trackPurchase";

	public const string IAP_CONFIG_GET = "/v1/iap/config";

	public const string IAP_COMPLETE_PURCHASE = "/v1/iap/purchase";

	public const string PRELOAD_FILE_CHECK = "/v1/preload/checkFile";

	public static string GetHPW()
	{
		return "bfid3Z53SFib325PJGFasae";
	}
}
