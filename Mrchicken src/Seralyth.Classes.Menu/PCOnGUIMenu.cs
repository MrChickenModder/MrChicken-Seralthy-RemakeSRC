using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json.Linq;

namespace Seralyth.Classes.Menu;

public class PCOnGUIMenu : MonoBehaviour
{
	private class MenuStatusEntry
	{
		public int actor;

		public string nickname;

		public string tab;

		public bool isOpen;
	}

	private struct TrailPoint
	{
		public Vector2 Position;

		public float Time;

		public float Hue;
	}

	private struct PlayerMacroStep
	{
		public float time;

		public Vector3 headPos;

		public Quaternion headRot;

		public Vector3 leftHandPos;

		public Vector3 rightHandPos;

		public bool leftGrab;

		public bool rightGrab;
	}

	private struct GlowBlob
	{
		public Vector2 Position;

		public float Time;

		public float Hue;

		public float Size;
	}

	[Serializable]
	private class AnnounceEntry
	{
		public string s;

		public string m;

		public int a;

		public long id;
	}

	[Serializable]
	private class AnnounceStorage
	{
		public List<AnnounceEntry> items = new List<AnnounceEntry>();
	}

	[Serializable]
	private class ReviewEntry
	{
		public string name;

		public int rating;

		public string comment;

		public string timestamp;
	}

	[Serializable]
	private class ReviewStorage
	{
		public List<ReviewEntry> items = new List<ReviewEntry>();
	}

	[Serializable]
	private class SuggestionEntry
	{
		public string sender;

		public string title;

		public string message;

		public string photoUrl;

		public int actor;
	}

	[Serializable]
	private class SuggestionStorage
	{
		public List<SuggestionEntry> items = new List<SuggestionEntry>();
	}

	[Serializable]
	private class PlayerDataFile
	{
		public List<PlayerDataEntry> notes = new List<PlayerDataEntry>();

		public List<PlayerDataEntry> lastSeen = new List<PlayerDataEntry>();

		public List<PlayerDataEntry> roles = new List<PlayerDataEntry>();
	}

	[Serializable]
	private class PlayerDataEntry
	{
		public string key;

		public string value;
	}

	private struct SupportReportEntry
	{
		public string sender;

		public string modName;

		public string description;

		public string screenshotUrl;

		public string timestamp;
	}

	public static PCOnGUIMenu Instance;

	public static bool IsOpen;

	private Rect guiRect = new Rect(0f, 0f, 700f, 430f);

	private bool showMods = true;

	private bool showPC;

	private bool showPlayers;

	private bool showPlayerColor;

	private bool showTheme;

	private bool showCredits;

	private bool showIcon;

	private bool showShowcases;

	private bool showCosmetics;

	private bool showAI;

	private bool showReview;

	private bool showGUISettings;

	private bool showGames;

	private bool showSupport;

	private bool enableRainbowSnake = PlayerPrefs.GetInt("GUI_RainbowSnake", 1) == 1;

	private bool enableMouseGlow = PlayerPrefs.GetInt("GUI_MouseGlow", 1) == 1;

	private int tooltipStyle = PlayerPrefs.GetInt("GUI_TooltipStyle", 0);

	private string typewriterTarget = "";

	private int typewriterChars;

	private float typewriterTimer;

	private Rect hoveredButtonRect;

	private string hoveredButtonOriginalLabel;

	private static readonly string[] tooltipStyleNames = new string[5] { "Normal", "Typewriter", "Fade In", "Button Label", "Button Label Typewriter" };

	private bool showWelcome = PlayerPrefs.GetString("SeralythWelcomeVersion", "") != "2026-09-29T13:10:37Z";

	private bool showAICmds;

	private int pcPageNumber;

	private static List<string> aiChatMessages = new List<string>();

	private static List<string> aiMessages = new List<string>();

	private static Vector2 aiChatScrollPosition;

	private static string aiChatInput = "";

	private static string aiInput = "";

	private static bool aiThinking;

	private static float aiThinkingTimer;

	private static int aiChatVersion = -1;

	private static float aiChatContentH;

	private static List<float> aiChatHeights = new List<float>();

	private static int aiVersion = -1;

	private static float aiContentH;

	private static List<float> aiHeights = new List<float>();

	private static bool aiScrollToBottom;

	private bool showSuggestions;

	private const byte MenuStatusByte = 83;

	private static List<MenuStatusEntry> menuStatusList = new List<MenuStatusEntry>();

	private Dictionary<string, List<PlayerMacroStep>> playerMacroStore = new Dictionary<string, List<PlayerMacroStep>>();

	private Dictionary<string, string> playerNotes = new Dictionary<string, string>();

	private Dictionary<string, string> playerLastSeen = new Dictionary<string, string>();

	private Dictionary<string, int> playerRoles = new Dictionary<string, int>();

	private static readonly Color roleFriendColor = new Color(0.3f, 0.9f, 0.4f);

	private static readonly Color roleFoeColor = new Color(0.95f, 0.25f, 0.25f);

	private static readonly string[] roleNames = new string[3] { "None", "Friend", "Foe" };

	private bool isRecordingPlayerMacro;

	private string recordingPlayerName = "";

	private Color recordingPlayerColor = Color.white;

	private List<PlayerMacroStep> currentRecordingSteps = new List<PlayerMacroStep>();

	private float macroRecordStartTime;

	private float macroLastRecordTime;

	private bool isPlayingPlayerMacro;

	private string playingMacroPlayerName = "";

	private int macroPlayIndex;

	private float macroPlayNextTime;

	private string macroPlaybackTarget = "";

	private string editingNoteFor = "";

	private string noteInputText = "";

	private bool showSearch;

	private string hoveredTooltip = "";

	private string searchText = "";

	private int reviewRating;

	private string reviewName = "";

	private string reviewComment = "";

	private string reviewSubmitResult = "";

	private float reviewSubmitTimer;

	private Vector2 reviewScrollPosition;

	private List<ReviewEntry> reviewEntries = new List<ReviewEntry>();

	private readonly List<TrailPoint> snakeTrail = new List<TrailPoint>();

	private float snakeProgress;

	private Texture2D snakeDot;

	private const int SnakeTrailLength = 20;

	private Texture2D blurGlow;

	private Texture2D roundedCornerTex;

	private const int CornerRadius = 10;

	private Vector2 smoothMousePos;

	private readonly List<GlowBlob> glowBlobs = new List<GlowBlob>();

	private float lastBlobTime;

	private string roomInput = "";

	private Vector2 scrollPosition;

	private Vector2 modScrollPosition;

	private Vector2 tourScrollPosition;

	private bool wasdEnabled;

	public float colorR = 1f;

	public float colorG = 1f;

	public float colorB = 1f;

	private float colorHue;

	private float colorSaturation = 1f;

	private float colorBrightness = 1f;

	private Texture2D colorWheelTexture;

	private const int ColorWheelSize = 180;

	private bool colorWheelDragging;

	private float themeWheelHue;

	private float themeWheelSaturation = 1f;

	private float themeWheelBrightness = 1f;

	private bool themeWheelDragging;

	private Texture2D themeBrightnessBar;

	private string customMenuTitle = "MrChicken Menu";

	private bool useCustomMenuTitle;

	private int playerColorTemplateIndex;

	private static readonly string[] playerColorTemplateNames = new string[7] { "Default", "Neon", "Pastel", "Matte", "Metallic", "Galaxy", "Monochrome" };

	private static readonly Color[][] playerColorTemplatePresets = new Color[7][]
	{
		(Color[])(object)new Color[2]
		{
			new Color(0.54f, 0.17f, 0.89f),
			new Color(1f, 0f, 1f)
		},
		(Color[])(object)new Color[2]
		{
			new Color(0f, 1f, 0.5f),
			new Color(1f, 0f, 0.5f)
		},
		(Color[])(object)new Color[2]
		{
			new Color(1f, 0.6f, 0.8f),
			new Color(0.6f, 0.8f, 1f)
		},
		(Color[])(object)new Color[2]
		{
			new Color(0.4f, 0.4f, 0.4f),
			new Color(0.6f, 0.6f, 0.6f)
		},
		(Color[])(object)new Color[2]
		{
			new Color(0.7f, 0.7f, 0.8f),
			new Color(0.9f, 0.9f, 1f)
		},
		(Color[])(object)new Color[2]
		{
			new Color(0.2f, 0f, 0.5f),
			new Color(0.8f, 0f, 1f)
		},
		(Color[])(object)new Color[2]
		{
			Color.black,
			Color.white
		}
	};

	private Texture2D brightnessBarTexture;

	private Vector2 playerColorScrollPosition;

	private bool showPlayerColorPresets = true;

	public float buttonSpacingY = 7f;

	private float lastKeyToggle;

	private int selectedModCategory = -1;

	private int currentCategoryIndex = -1;

	private static int lastSyncedThemeType = -1;

	private static string[] modCategoryNames;

	private static int[] modCategoryIndices;

	private Vector2 modCategoryScrollPosition;

	private Vector2 cosmeticScrollPosition;

	private int selectedCosmeticCategory = -1;

	private Vector2 guiScrollPosition;

	private string[] ttb = new string[9] { "", "", "", "", "", "", "", "", "" };

	private int ttWinner;

	private int ttTurn;

	private int ttScoreX;

	private int ttScoreO;

	private int ttScoreD;

	private int ttDiff;

	private float ttAICooldown;

	private int ttLineA = -1;

	private int ttLineB = -1;

	private Texture2D ttLineTex;

	private bool ttPlayerIsX = true;

	private string ttAISym = "O";

	private string ttPlayerSym = "X";

	private int gameMode;

	private static readonly string[] gameNames = new string[50]
	{
		"Tic Tac Toe", "Wordle", "Block Blast", "Snake", "Connect Four", "Flappy Bird", "Minesweeper", "2048", "Pong", "Simon Says",
		"Hangman", "Memory Match", "Checkers", "Sudoku", "Tower Defense", "Maze", "Breakout", "MS Hard", "Chinese Checkers", "Tetris",
		"Solitaire", "Chess", "Whack-a-Mole", "Reaction Test", "Typing Speed", "Catch Objects", "Pacman", "Tank Battle", "Battleship", "Yahtzee",
		"Color Match", "Pipe Puzzle", "Lights Out", "Nonogram", "Rock Paper Scissors", "Number Guess", "Dice Roll", "Coin Flip", "Blackjack", "Gomoku",
		"Dots and Boxes", "Checkers 2P", "Sliding Puzzle", "Bulls and Cows", "FreeCell", "Tron", "Bomberman", "Brick Calculator", "Othello", "Rush Hour"
	};

	private Vector2 gameScrollPosition;

	private bool showGameHelp;

	private static readonly string[] gameHelp = new string[50]
	{
		"Place X or O in a row of 3 to win. Play vs AI with 3 difficulty levels.", "Guess the 5-letter word in 6 tries. Green=correct, Yellow=wrong spot, Grey=not in word.", "Fit falling shapes to fill rows. Complete rows to clear them. Combo bonus for multi-row clears.", "Use arrows/WASD to guide the snake. Eat food to grow. Don't hit walls or yourself!", "Drop discs into columns. Get 4 in a row (horizontal, vertical, or diagonal) to win.", "Click/tap to flap. Avoid pipes and ground. How many can you score?", "Click to reveal cells. Numbers show adjacent mines. Flag suspected mines. Clear board to win.", "Slide tiles by swiping/clicking. Combine matching numbers to reach 2048!", "Mouse or W/S for your paddle. First to 11 wins. Ball speeds up each rally.", "Watch the pattern, then repeat it. Sequence grows each round. How far can you get?",
		"Guess the word letter by letter. 6 wrong guesses and the hangman is complete.", "Flip cards to find matching pairs. Remember positions! Match all to win.", "Move diagonally and jump opponent pieces. King pieces at the back row. Capture all to win.", "Fill the 9x9 grid with numbers 1-9. No repeats in rows, columns, or 3x3 boxes.", "Defend your base from waves of enemies. Earn gold to buy/upgrade towers.", "Navigate the maze from start to finish. Use arrows/WASD. Maze regenerates each game.", "Bounce a ball to break bricks. Don't let the ball fall past your paddle!", "Hard mode Minesweeper: 16x16 grid with 40 mines. Flag mode toggle included.", "Get all your pieces to the opposite corner. Simplified strategy board game.", "Rotate and drop falling pieces (tetrominoes). Complete rows to clear them. How high can you stack?",
		"Classic card game: build 4 foundation piles from Ace to King by suit.", "Full chess game vs AI. Click piece then destination. AI auto-moves after you.", "Click moles when they pop up! Score 10 per whack. 5 lives - miss and lose one.", "Wait for the screen to turn green, then click as fast as you can! Measure your reaction time.", "Type the displayed words as fast as you can. 60-second test measures your WPM.", "Catch falling bananas in your basket! Move with mouse or A/D keys.", "Navigate the maze eating dots. Avoid ghosts! 3 lives.", "WASD to move your tank. Click to shoot incoming enemies. Don't let them reach you!", "Place 5 ships, then fire on the enemy grid. Sink all their ships before they sink yours!", "Roll and hold dice to score combinations. Yahtzee=5 of a kind (50pts). Use all 13 categories.",
		"Match the target color using RGB sliders! Submit your color before time runs out.", "Click pipes to rotate them. Connect all pipes to solve the puzzle.", "Toggle lights and their neighbors. Turn all lights OFF to solve! Fewest moves = best.", "Logic puzzle: use row/column clues to fill cells. Left-click=fill, Right-click=mark X.", "Pick Rock, Paper, or Scissors. Beat the AI! Track wins, losses, and streaks.", "Guess a number between 1 and 100. Get hints: too high or too low. Fewest guesses wins!", "Roll 5 dice up to 3 times per turn. Hold dice between rolls. Score combos for points!", "Flip a coin! Track your streak of heads or tails in a row. How high can you go?", "Beat the dealer to 21 without going over! Hit to draw, Stand to hold. Ace=1 or 11.", "Five in a row on a 15x15 grid. Click to place your stone. Beat the AI!",
		"Click edges between dots to claim lines. Complete a box to earn a point and go again.", "Two-player checkers on the same board. Take turns moving and jumping opponent pieces.", "Slide tiles into empty space to arrange numbers 1-15 in order. Fewest moves = best!", "Guess the 4-digit secret code. Bulls = correct digit & position. Cows = correct digit, wrong spot.", "Build 4 foundation piles by suit. Move cards between columns. Click stock to draw cards.", "Both you and AI leave light trails on a grid. Don't crash into walls or trails!", "Place bombs to destroy blocks and avoid enemies. Collect powerups. Clear all enemies!", "Count numbers on colored bricks to find safe ones. Avoid hidden bombs! Clear the board.", "Place stones to flip opponent pieces to your color. Surround chains to flip them!", "Slide cars to clear a path for the red car to exit. Think strategically!"
	};

	private string wdTarget = "";

	private string[] wdGuesses = new string[6] { "", "", "", "", "", "" };

	private int[,] wdColors = new int[6, 5];

	private int wdRow;

	private string wdInput = "";

	private int wdGuessesWon;

	private int wdGuessesLost;

	private bool wdResultCounted;

	private int wdHintsUsed;

	private string wdHintText;

	private List<int> wdUsedHintIndices = new List<int>();

	private static readonly Vector2Int[][] bbShapeDefs = new Vector2Int[15][]
	{
		(Vector2Int[])(object)new Vector2Int[1]
		{
			new Vector2Int(0, 0)
		},
		(Vector2Int[])(object)new Vector2Int[2]
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0)
		},
		(Vector2Int[])(object)new Vector2Int[2]
		{
			new Vector2Int(0, 0),
			new Vector2Int(0, 1)
		},
		(Vector2Int[])(object)new Vector2Int[3]
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0),
			new Vector2Int(2, 0)
		},
		(Vector2Int[])(object)new Vector2Int[3]
		{
			new Vector2Int(0, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, 2)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(1, 1)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0),
			new Vector2Int(2, 0),
			new Vector2Int(1, 1)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, 2),
			new Vector2Int(1, 2)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(1, 1),
			new Vector2Int(2, 1)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, 1),
			new Vector2Int(1, 1),
			new Vector2Int(1, 0),
			new Vector2Int(2, 0)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0),
			new Vector2Int(1, 1),
			new Vector2Int(2, 1)
		},
		(Vector2Int[])(object)new Vector2Int[5]
		{
			new Vector2Int(0, 0),
			new Vector2Int(0, 2),
			new Vector2Int(1, 0),
			new Vector2Int(1, 1),
			new Vector2Int(1, 2)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0),
			new Vector2Int(2, 0),
			new Vector2Int(3, 0)
		},
		(Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, 2),
			new Vector2Int(0, 3)
		},
		(Vector2Int[])(object)new Vector2Int[5]
		{
			new Vector2Int(0, 1),
			new Vector2Int(1, 0),
			new Vector2Int(1, 1),
			new Vector2Int(1, 2),
			new Vector2Int(2, 1)
		}
	};

	private static readonly Color[] bbBlockColors = (Color[])(object)new Color[8]
	{
		new Color(0.95f, 0.3f, 0.3f),
		new Color(0.3f, 0.8f, 0.3f),
		new Color(0.3f, 0.5f, 0.95f),
		new Color(0.95f, 0.7f, 0.2f),
		new Color(0.8f, 0.3f, 0.9f),
		new Color(0.2f, 0.85f, 0.85f),
		new Color(0.95f, 0.95f, 0.3f),
		new Color(0.95f, 0.5f, 0.7f)
	};

	private int[,] bbGrid = new int[8, 8];

	private int[] bbShapeTypes = new int[3];

	private int[] bbShapeColors = new int[3];

	private bool[] bbShapePlaced = new bool[3];

	private int bbSelectedShape = -1;

	private int bbScore;

	private int bbBestScore;

	private bool bbGameActive;

	private bool bbGameOver;

	private float bbClearAnimTime;

	private float bbComboTextTime;

	private string bbComboText = "";

	private float bbPopupTime;

	private int bbPopupScore;

	private float bbPopupX;

	private float bbPopupY;

	private Vector2 bbScrollPosition;

	private bool bbDragging;

	private int bbDragPiece = -1;

	private const int SnakeGridW = 20;

	private const int SnakeGridH = 15;

	private int[,] snakeGrid;

	private List<Vector2Int> snakeBody;

	private Vector2Int snakeDir;

	private Vector2Int snakeFood;

	private int snakeScore;

	private int snakeBestScore;

	private bool snakeGameActive;

	private bool snakeAlive;

	private float snakeMoveTimer;

	private float snakeMoveInterval = 0.25f;

	private bool snakeUseAI;

	private Vector2 snakeScrollPos;

	private List<Vector2Int> snakePath;

	private const int C4Cols = 7;

	private const int C4Rows = 6;

	private int[,] c4Grid;

	private int c4Winner;

	private int c4Turn;

	private int c4Diff;

	private int c4ScoreX;

	private int c4ScoreO;

	private float c4AICooldown;

	private Vector2 c4ScrollPos;

	private float fbBirdY;

	private float fbBirdVel;

	private float fbBirdX;

	private List<float> fbPipeX;

	private List<float> fbPipeGap;

	private int fbScore;

	private int fbBestScore;

	private bool fbGameActive;

	private bool fbAlive;

	private Vector2 fbScrollPos;

	private float fbGroundOffset;

	private static Texture2D[] gameBackgrounds;

	private static bool gameBgsLoaded;

	private static readonly string[] gameBgPaths = new string[8] { "C:\\Users\\kalew\\OneDrive\\Pictures\\image.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\image_8cc3738.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Photo_25_12-16_17_53_26_91.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\IMG_7678.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\file_000000002fe871f79ea7db10eb58a212.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\IMG_0272.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\download.jfif", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-03-10 145740.png" };

	private static readonly string[] mmImagePaths = new string[8] { "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-05-09 104133.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-07-03 165642.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-06-20 151624.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-07-02 160316.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-06-10 031843.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-06-05 191823.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-06-10 035008.png", "C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\Screenshot 2026-06-10 035016.png" };

	private static Texture2D[] mmTextures;

	private static bool mmTexturesLoaded;

	private static int gameBgLoadIndex;

	private static bool gameBgLoadStarted;

	private int msRows = 10;

	private int msCols = 10;

	private int msMines = 15;

	private int[,] msGrid;

	private bool[,] msRevealed;

	private bool[,] msFlagged;

	private bool msGameOver;

	private bool msWon;

	private int msFlagsLeft;

	private float msTimer;

	private bool msStarted;

	private bool msFlagMode;

	private Vector2 msScrollPos;

	private int[,] g4Grid;

	private int g4Score;

	private int g4Best;

	private bool g4Active;

	private bool g4Won;

	private float pongBallX;

	private float pongBallY;

	private float pongBallVX;

	private float pongBallVY;

	private float pongPlayerY;

	private float pongEnemyY;

	private int pongPScore;

	private int pongEScore;

	private bool pongStarted;

	private float pongFieldW = 400f;

	private float pongFieldH = 260f;

	private int[] simonPattern;

	private int simonPI;

	private int simonLen;

	private int simonPhase;

	private int simonScore;

	private int simonHigh;

	private float simonTimer;

	private bool simonFlash;

	private int simonFlashI;

	private int simonFlashCount;

	private static readonly Color[] simonCols = (Color[])(object)new Color[4]
	{
		new Color(0.9f, 0.2f, 0.2f),
		new Color(0.2f, 0.7f, 0.2f),
		new Color(0.2f, 0.4f, 0.9f),
		new Color(0.9f, 0.8f, 0.1f)
	};

	private static readonly string[] simonNames = new string[4] { "Red", "Green", "Blue", "Yellow" };

	private string hmWord = "";

	private string hmGuesses = "";

	private int hmWrong;

	private bool hmWon;

	private bool hmLost;

	private Vector2 hmScrollPos;

	private int[,] mmGrid;

	private bool[,] mmOpen;

	private bool[,] mmMatched;

	private int mmR1 = -1;

	private int mmC1 = -1;

	private int mmR2 = -1;

	private int mmC2 = -1;

	private bool mmBusy;

	private int mmPairs;

	private int mmMoves;

	private bool mmDone;

	private float mmTimer;

	private float mmFlipBack;

	private int[,] ckBoard;

	private int ckTurn;

	private int ckSelR = -1;

	private int ckSelC = -1;

	private bool ckGameOver;

	private int ckWinner;

	private Vector2 ckScrollPos;

	private static readonly int[] ckDirR = new int[2] { -1, -1 };

	private static readonly int[] ckDirC = new int[2] { -1, 1 };

	private int[,] sdkGrid;

	private int[,] sdkSol;

	private bool[,] sdkFixed;

	private int sdkSelR = -1;

	private int sdkSelC = -1;

	private int sdkMistakes;

	private Vector2 sdkScrollPos;

	private float[,,] tdMap;

	private float tdTimer;

	private int tdWave;

	private int tdLives;

	private int tdGold;

	private int tdSelTow;

	private bool tdActive;

	private List<Vector4> tdEnemies;

	private List<float> tdEnemyHP;

	private List<float> tdEnemyMaxHP;

	private float tdSpawnTimer;

	private int tdSpawned;

	private int tdWaveEnemies;

	private float tdGoldTimer;

	private int tdSelectedTowerType;

	private List<float[,,]> tdTowers;

	private static readonly float[] tdTowCost = new float[3] { 50f, 100f, 150f };

	private static readonly float[] tdTowRange = new float[3] { 60f, 80f, 100f };

	private static readonly float[] tdTowDmg = new float[3] { 15f, 30f, 50f };

	private static readonly float[] tdTowRate = new float[3] { 1f, 0.7f, 0.5f };

	private static readonly string[] tdTowNames = new string[3] { "Blaster ($50)", "Cannon ($100)", "Sniper ($150)" };

	private int mzW = 12;

	private int mzH = 8;

	private int[,] mzWalls;

	private int mzPR;

	private int mzPC;

	private int mzER;

	private int mzEC;

	private bool mzDone;

	private bool mzGenerated;

	private Vector2 mzScrollPos;

	private float brPaddleX;

	private float brBallX;

	private float brBallY;

	private float brBallVX;

	private float brBallVY;

	private bool[,] brBricks;

	private int brLives;

	private int brScore;

	private bool brActive;

	private float brFieldW = 400f;

	private float brFieldH = 300f;

	private int mshRows = 16;

	private int mshCols = 16;

	private int mshMines = 40;

	private int[,] mshGrid;

	private bool[,] mshRevealed;

	private bool[,] mshFlagged;

	private bool mshGameOver;

	private bool mshWon;

	private int mshFlagsLeft;

	private float mshTimer;

	private bool mshStarted;

	private bool mshFlagMode;

	private Vector2 mshScrollPos;

	private int[,] tetGrid;

	private int tetPiece;

	private int tetRot;

	private int tetX;

	private int tetY;

	private int tetNext;

	private int tetScore;

	private bool tetActive;

	private float tetTimer;

	private float tetSpeed;

	private static readonly int[][,] tetShapes = new int[7][,]
	{
		new int[1, 4] { { 1, 1, 1, 1 } },
		new int[2, 2]
		{
			{ 1, 1 },
			{ 1, 1 }
		},
		new int[2, 3]
		{
			{ 0, 1, 0 },
			{ 1, 1, 1 }
		},
		new int[2, 3]
		{
			{ 1, 0, 0 },
			{ 1, 1, 1 }
		},
		new int[2, 3]
		{
			{ 0, 0, 1 },
			{ 1, 1, 1 }
		},
		new int[2, 3]
		{
			{ 1, 1, 0 },
			{ 0, 1, 1 }
		},
		new int[2, 3]
		{
			{ 0, 1, 1 },
			{ 1, 1, 0 }
		}
	};

	private static readonly Color[] tetColors = (Color[])(object)new Color[7]
	{
		Color.cyan,
		Color.yellow,
		Color.magenta,
		Color.blue,
		new Color(1f, 0.5f, 0f),
		Color.red,
		Color.green
	};

	private List<int>[] solColumns;

	private List<bool>[] solColFaceUp;

	private List<int>[] solFoundation;

	private List<int> solStock;

	private int solWaste;

	private bool solWasteActive;

	private List<int> solWastePile;

	private int solSelectedCol = -1;

	private int solSelectedIdx = -1;

	private int[,] ccBoard;

	private int ccSelR = -1;

	private int ccSelC = -1;

	private int ccMoves;

	private bool ccGameOver;

	private int ccBoardSize = 11;

	private int ccPlayerPieces;

	private int ccAIPieces;

	private int[,] chBoard;

	private int chTurn;

	private int chSelR = -1;

	private int chSelC = -1;

	private bool chGameOver;

	private int chWinner;

	private int[,] wamGrid;

	private float wamTimer;

	private float wamSpawnTimer;

	private int wamScore;

	private int wamLives;

	private bool wamActive;

	private float wamMoleTimer;

	private int rtState;

	private float rtTimer;

	private float rtBest;

	private float rtWaitTime;

	private string[] tstWords;

	private string tstCurrentWord;

	private string tstTyped;

	private int tstCorrect;

	private int tstTotal;

	private float tstTimer;

	private bool tstActive;

	private int tstWPM;

	private float tstStartTime;

	private float coBasketX;

	private float coFieldW = 400f;

	private float coFieldH = 300f;

	private List<Vector3> coFalling;

	private int coScore;

	private int coLives;

	private bool coActive;

	private float coSpawnTimer;

	private float coSpeed;

	private int[,] pacMaze;

	private int pacPR;

	private int pacPC;

	private int pacDir;

	private int pacScore;

	private bool pacActive;

	private int pacLives;

	private float pacGhostTimer;

	private List<int[]> pacGhosts;

	private List<int> pacGhostDirs;

	private float pacMoveTimer;

	private float tbPX;

	private float tbPY;

	private List<Vector4> tbEnemies;

	private List<float> tbEnemyHP;

	private List<Vector3> tbBullets;

	private List<bool> tbBulletPlayer;

	private int tbScore;

	private int tbLives;

	private bool tbActive;

	private float tbSpawnTimer;

	private float tbShootCooldown;

	private int[,] bsPlayerBoard;

	private int[,] bsEnemyBoard;

	private bool[,] bsPlayerShips;

	private bool[,] bsEnemyShips;

	private int bsPhase;

	private int bsSelR = -1;

	private int bsSelC = -1;

	private int bsPlacingShip;

	private bool bsPlacingH;

	private int bsPlayerHits;

	private int bsEnemyHits;

	private bool bsGameOver;

	private int[] yzDice;

	private bool[] yzHeld;

	private int yzRerolls;

	private int[] yzScores;

	private bool[] yzUsed;

	private int yzTotal;

	private bool yzGameOver;

	private Color cmTarget;

	private Color cmPlayer;

	private int cmScore;

	private float cmTimer;

	private bool cmActive;

	private float cmSliderR;

	private float cmSliderG;

	private float cmSliderB;

	private int[,] ppGrid;

	private int[,] ppRotation;

	private int ppW = 6;

	private int ppH = 6;

	private bool ppSolved;

	private bool[,] loGrid;

	private int loMoves;

	private int loSize = 5;

	private int[,] nnpGrid;

	private int[,] nnpSolution;

	private int nnpW = 5;

	private int nnpH = 5;

	private List<int>[] nnpRowClues;

	private List<int>[] nnpColClues;

	private bool nnpSolved;

	private int rpsPlayerChoice = -1;

	private int rpsAIChoice = -1;

	private int rpsWins;

	private int rpsLosses;

	private int rpsDraws;

	private int rpsStreak;

	private string rpsResult = "";

	private float rpsAnimTime;

	private int ngTarget;

	private int ngGuess;

	private int ngAttempts;

	private int ngBest = -1;

	private string ngHint = "";

	private bool ngWon;

	private string ngInput = "";

	private int[] drDice = new int[5];

	private bool[] drHeld = new bool[5];

	private int drRerolls;

	private int drScore;

	private int drBestScore;

	private bool drRolled;

	private int cfResult = -1;

	private int cfStreak;

	private string cfStreakType = "";

	private int cfTotal;

	private bool cfFlipping;

	private float cfAnimTime;

	private Texture2D cfCoinTex;

	private List<int> bjPlayerHand = new List<int>();

	private List<int> bjDealerHand = new List<int>();

	private List<string> bjPlayerLabels = new List<string>();

	private List<string> bjDealerLabels = new List<string>();

	private int bjBet;

	private int bjChips;

	private bool bjDealerHidden;

	private bool bjGameOver;

	private string bjResult = "";

	private bool bjBetting;

	private int[,] gmBoard;

	private int gmTurn;

	private int gmWinner;

	private int gmWinR1;

	private int gmWinC1;

	private int gmWinR2;

	private int gmWinC2;

	private Vector2 gmScrollPos;

	private bool gmAIThinking;

	private int dbRows = 5;

	private int dbCols = 5;

	private bool[,] dbHoriz;

	private bool[,] dbVert;

	private int[,] dbBoxes;

	private int dbTurn;

	private int dbScore1;

	private int dbScore2;

	private bool dbGameOver;

	private int[,] ck2Board;

	private int ck2Turn;

	private int ck2SelR = -1;

	private int ck2SelC = -1;

	private bool ck2GameOver;

	private int ck2Winner;

	private int[,] spGrid;

	private int spSize = 4;

	private int spMoves;

	private bool spSolved;

	private int spBest;

	private int[] bucSecret = new int[4];

	private int[] bucGuessArr = new int[4];

	private int bucAttempt;

	private int bucMaxAttempts = 10;

	private int bucBulls;

	private int bucCows;

	private bool bucWon;

	private string bucInput = "";

	private List<string> bucHistory = new List<string>();

	private List<int>[] fcColumns;

	private List<bool>[] fcColFaceUp;

	private List<int>[] fcFoundation;

	private List<int> fcStock;

	private int fcWaste;

	private bool fcWasteActive;

	private int fcSelectedCol = -1;

	private int fcSelectedIdx = -1;

	private int trSize = 20;

	private int[,] trGrid;

	private int trPR;

	private int trPC;

	private int trER;

	private int trEC;

	private int trPDir;

	private int trEDir;

	private bool trAlive;

	private bool trActive;

	private int trScore;

	private int bmSize = 11;

	private int[,] bmGrid;

	private int bmPR;

	private int bmPC;

	private int bmBombs;

	private int bmRange;

	private int bmLives;

	private int bmScore;

	private bool bmActive;

	private List<int[]> bmEnemies;

	private float bmEnemyTimer;

	private float bmBombTimer;

	private bool bmBombPlaced;

	private int bmBombR;

	private int bmBombC;

	private int brcSize = 8;

	private int[,] brcGrid;

	private bool[,] brcRevealed;

	private bool[,] brcFlagged;

	private bool brcGameOver;

	private bool brcWon;

	private int brcBombs;

	private int brcFlagsLeft;

	private bool brcFlagMode;

	private int[,] othBoard;

	private int othTurn;

	private int othWinner;

	private bool othGameOver;

	private Vector2 othScrollPos;

	private int rhSize = 6;

	private int[,] rhGrid;

	private int rhCars;

	private int[] rhCarR;

	private int[] rhCarC;

	private int[] rhCarLen;

	private int[] rhCarDir;

	private int rhSelected = -1;

	private int rhMoves;

	private bool rhSolved;

	private static List<string> onlinePlayers = new List<string>();

	private static bool playersInited;

	private int selectedPlayerIndex = -1;

	private int camMode;

	private float videoCamTimer;

	private int playerInfoPage;

	private Camera fpCamera;

	private RenderTexture fpRenderTexture;

	private Camera mirrorCamera;

	private RenderTexture mirrorRenderTexture;

	private Camera portraitCamera;

	private RenderTexture portraitRenderTexture;

	private Texture2D playerPortrait;

	private string playerPortraitName = "";

	private Rect portraitWindowRect = new Rect(710f, 0f, 280f, 320f);

	private Rect cosmeticsWindowRect = new Rect(-290f, 0f, 280f, 320f);

	private Rect mirrorWindowRect = new Rect(710f, 0f, 280f, 320f);

	private Vector2 cosmeticsScrollPos;

	private Texture2D selfPortrait;

	private bool selfPortraitCaptured;

	private GUIStyle fpNameStyle;

	private bool showTour;

	private bool tourComplete;

	private bool showChat;

	private bool showFriends;

	private Vector2 friendsScrollPosition;

	private string selectedFriendKey = "";

	private string friendsChatInput = "";

	private bool friendsNeedsRefresh = true;

	private int tourIndex;

	private Rect tourAnimRect;

	private int tourPrevIndex = -1;

	private float tourAnimTime;

	private const float TourAnimDuration = 0.7f;

	private float tourOverlayX;

	private float tourOverlayY;

	private float tourOverlayTargetX;

	private float tourOverlayTargetY;

	private float tourFingerX;

	private float tourFingerY;

	private float tourFingerTargetX;

	private float tourFingerTargetY;

	private float tourFingerClickTime;

	private static Texture2D tourCursorTex;

	private string[] tourSteps = new string[24]
	{
		"Browse mod categories on the left and click one to see its mods", "Toggle individual mods on and off from the main list", "Click Back to Tabs at the bottom right to return to the main menu", "Click Mods in the sidebar to browse and toggle mods by category", "Click PC to change your nickname, join rooms, or use WASD fly", "Click Players to see who is currently in your room", "Click Chat to send and receive messages with other players", "Switch to Announce in the Chat tab to view admin announcements (owners/admins only can send)", "Click Player Color to change your gorilla's color", "Click Theme to customize the GUI background and button colors",
		"Click Credits for links to GitHub and the Discord server", "Click Icon to change the color of the menu icon", "Click Showcases to watch video showcases", "Click Cosmetics to view and toggle your owned cosmetics on and off", "Click Suggestions to submit feedback or browse suggestions from other players", "Click AI to type mod names to toggle them on and off", "The DISCONNECT button at the bottom leaves the current room", "The Search button at the top filters mods by name", "Use Insert key to toggle the GUI on and off", "Drag the title bar to move the GUI window",
		"Use the page buttons to navigate between pages of mods, or enable Page Scrolling for joystick scroll", "Click Review to rate the menu and check reviews from other players", "Click GUI Settings to toggle visual effects like the rainbow border and mouse glow", "Scroll down the sidebar to find Games with Tic Tac Toe and Wordle"
	};

	private Rect[] tourTargets = (Rect[])(object)new Rect[24]
	{
		new Rect(5f, 21f, 155f, 374f),
		new Rect(170f, 80f, 525f, 310f),
		new Rect(540f, 370f, 150f, 25f),
		new Rect(5f, 21f, 150f, 25f),
		new Rect(5f, 48f, 150f, 25f),
		new Rect(5f, 75f, 150f, 25f),
		new Rect(5f, 102f, 150f, 25f),
		new Rect(170f, 21f, 80f, 22f),
		new Rect(5f, 129f, 150f, 25f),
		new Rect(5f, 156f, 150f, 25f),
		new Rect(5f, 183f, 150f, 25f),
		new Rect(5f, 210f, 150f, 25f),
		new Rect(5f, 237f, 150f, 25f),
		new Rect(5f, 264f, 150f, 25f),
		new Rect(5f, 291f, 150f, 25f),
		new Rect(5f, 318f, 150f, 25f),
		new Rect(0f, 400f, 700f, 25f),
		new Rect(600f, 21f, 95f, 25f),
		new Rect(540f, 2f, 155f, 22f),
		new Rect(165f, 2f, 200f, 22f),
		new Rect(180f, 370f, 400f, 25f),
		new Rect(5f, 345f, 150f, 25f),
		new Rect(5f, 372f, 150f, 25f),
		new Rect(5f, 399f, 150f, 25f)
	};

	private List<string> chatMessages = new List<string>();

	private string chatInput = "";

	private Vector2 chatScrollPosition;

	private int prevChatCount;

	private const byte ChatByte = 80;

	private const byte AnnounceByte = 81;

	private const byte AnnounceDeleteByte = 82;

	private const int chatMaxMessages = 100;

	private const int announceMaxMessages = 100;

	private const string AnnounceRoomPropKey = "SeralythAnnc";

	private List<AnnounceEntry> announceData = new List<AnnounceEntry>();

	private string announceInput = "";

	private bool showAnnouncements;

	private int prevAnnounceCount;

	private string cachedAnnounceJson = "";

	private long announceIdCounter;

	private Vector2 sidebarScrollPosition;

	private int prevCategoryStep = -1;

	private const byte SuggestionByte = 82;

	private string suggestionTitle = "";

	private string suggestionMessage = "";

	private string suggestionPhotoUrl = "";

	private string suggestionStatus = "";

	private bool showSuggestionForm = true;

	private List<SuggestionEntry> suggestionList = new List<SuggestionEntry>();

	private Vector2 suggestionListScroll;

	private static readonly string[] hmWords = new string[50]
	{
		"computer", "gorilla", "banana", "jungle", "forest", "moon", "rocket", "planet", "galaxy", "ocean",
		"mountain", "river", "castle", "dragon", "wizard", "puzzle", "guitar", "piano", "basket", "blanket",
		"dolphin", "eclipse", "feather", "harvest", "lantern", "magnet", "nebula", "parrot", "shadow", "tunnel",
		"wizard", "zephyr", "anchor", "blaze", "cactus", "dagger", "emerald", "falcon", "glacier", "horizon",
		"ivory", "jasper", "knight", "legend", "mirage", "nectar", "oracle", "phoenix", "quartz", "ripple"
	};

	private float wasdSpeed = 1f;

	private float wasdRotation = 1f;

	private float wasdJump = 1f;

	public static Color guiBgColor = Color.white;

	public static Color guiContentColor = Color.blue;

	public static Color guiColorA = new Color(1f, 0f, 1f);

	public static Color guiColorB = new Color(0.54f, 0.17f, 0.89f);

	public static Color guiIconColor = Color.white;

	private static bool isRainbowTheme;

	private static float rainbowTime;

	private static Texture2D menuIconTexture;

	private static GUIStyle titleStyle;

	private static Texture2D gradientTexture;

	private Vector2 supportScrollPosition;

	private string supportLookupInput = "";

	private string supportLookupResult = "";

	private Color supportLookupColor = Color.white;

	private string reportModName = "";

	private string reportDescription = "";

	private string reportScreenshotUrl = "";

	private string reportStatus = "";

	private float reportStatusTimer;

	private List<SupportReportEntry> reportEntries = new List<SupportReportEntry>();

	private Vector2 reportListScroll;

	private static string PlayerDataPath => Path.Combine(Application.persistentDataPath, "SeralythPlayerData.json");

	private static string LocalReviewPath => Path.Combine(Application.persistentDataPath, "SeralythReviews.json");

	private static string LocalAnnouncePath => Path.Combine(Application.persistentDataPath, "SeralythAnnc.json");

	private static string SuggestionSavePath => Path.Combine(Application.persistentDataPath, "SeralythSuggestions.json");

	private static string GenerateRandomWord()
	{
		string[] array = new string[18]
		{
			"b", "c", "d", "f", "g", "h", "j", "k", "l", "m",
			"n", "p", "r", "s", "t", "v", "w", "y"
		};
		string[] array2 = new string[5] { "a", "e", "i", "o", "u" };
		string[] array3 = new string[8] { "cvccv", "vcvcc", "cvcvc", "cvvcv", "vccvc", "ccvcc", "cvcvc", "vcvvc" };
		string text = array3[Random.Range(0, array3.Length)];
		string text2 = "";
		string text3 = text;
		foreach (char c in text3)
		{
			text2 += ((c == 'c') ? array[Random.Range(0, array.Length)] : array2[Random.Range(0, array2.Length)]);
		}
		return text2;
	}

	private void InitPlayers()
	{
		if (playersInited || (Object)(object)NetworkSystem.Instance == (Object)null)
		{
			return;
		}
		playersInited = true;
		LoadPlayerData();
		NetworkSystem instance = NetworkSystem.Instance;
		instance.OnJoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)instance.OnJoinedRoomEvent + (Action)delegate
		{
			onlinePlayers.Clear();
			cachedAnnounceJson = "";
			onlinePlayers.Add(NetworkSystem.Instance.LocalPlayer.NickName + " (you)");
			NetPlayer[] playerListOthers2 = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val2 in playerListOthers2)
			{
				onlinePlayers.Add(val2.NickName);
				UpdateLastSeen(val2.NickName);
			}
			UpdateLastSeen(NetworkSystem.Instance.LocalPlayer.NickName);
		};
		NetworkSystem instance2 = NetworkSystem.Instance;
		instance2.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)instance2.OnPlayerJoined + (Action<NetPlayer>)delegate(NetPlayer p)
		{
			if (p != NetworkSystem.Instance.LocalPlayer)
			{
				onlinePlayers.Add(p.NickName);
				UpdateLastSeen(p.NickName);
			}
		};
		NetworkSystem instance3 = NetworkSystem.Instance;
		instance3.OnPlayerLeft = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)instance3.OnPlayerLeft + (Action<NetPlayer>)delegate(NetPlayer p)
		{
			onlinePlayers.Remove(p.NickName);
		};
		NetworkSystem instance4 = NetworkSystem.Instance;
		instance4.OnReturnedToSinglePlayer = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)instance4.OnReturnedToSinglePlayer + (Action)delegate
		{
			onlinePlayers.Clear();
		};
		if (NetworkSystem.Instance.InRoom)
		{
			onlinePlayers.Clear();
			onlinePlayers.Add(NetworkSystem.Instance.LocalPlayer.NickName + " (you)");
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				onlinePlayers.Add(val.NickName);
			}
		}
	}

	public static void Enable()
	{
		IsOpen = true;
		if ((Object)(object)Instance != (Object)null)
		{
			Instance.BroadcastMenuStatus();
		}
	}

	public static void Disable()
	{
		IsOpen = false;
		if ((Object)(object)Instance != (Object)null)
		{
			Instance.BroadcastMenuStatus();
		}
	}

	private static void BuildCategoryList()
	{
		if (Buttons.buttons == null || Buttons.buttons.Length == 0 || Buttons.buttons[0] == null)
		{
			return;
		}
		ButtonInfo[] array = Buttons.buttons[0].Where((ButtonInfo b) => b != null && !b.buttonText.StartsWith("Exit ") && b.buttonText != "Join Discord" && b.buttonText != "configuration" && !b.label).ToArray();
		modCategoryNames = new string[array.Length];
		modCategoryIndices = new int[array.Length];
		for (int num = 0; num < array.Length; num++)
		{
			string buttonText = array[num].buttonText;
			modCategoryNames[num] = buttonText;
			string exitText = "Exit " + buttonText;
			int num2 = -1;
			for (int num3 = 0; num3 < Buttons.buttons.Length; num3++)
			{
				if (Buttons.buttons[num3] != null && Buttons.buttons[num3].Any((ButtonInfo b) => b != null && b.buttonText == exitText))
				{
					num2 = num3;
					break;
				}
			}
			modCategoryIndices[num] = ((num2 >= 0) ? num2 : num);
		}
	}

	private void SyncFromVrTheme()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (Main.backgroundColor != null)
		{
			guiBgColor = Main.backgroundColor.GetColor(0);
		}
		if (Main.textColors != null && Main.textColors.Length > 1)
		{
			guiContentColor = Main.textColors[1].GetColor(0);
		}
		if (Main.buttonColors != null && Main.buttonColors.Length != 0)
		{
			guiColorA = Main.buttonColors[0].GetColor(0);
		}
		if (Main.buttonColors != null && Main.buttonColors.Length > 1)
		{
			guiColorB = Main.buttonColors[1].GetColor(0);
		}
	}

	private void OnGUI()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Invalid comparison between Unknown and I4
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Expected O, but got Unknown
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Expected O, but got Unknown
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Invalid comparison between Unknown and I4
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Expected O, but got Unknown
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 5 && (int)Event.current.keyCode == 277 && Time.time - lastKeyToggle > 0.2f)
		{
			lastKeyToggle = Time.time;
			IsOpen = !IsOpen;
		}
		if ((int)Event.current.type == 5 && (int)Event.current.keyCode == 308 && Time.time - lastKeyToggle > 0.2f)
		{
			lastKeyToggle = Time.time;
			AdminTagGUI.Toggle();
		}
		if ((int)Event.current.type == 5 && (int)Event.current.keyCode == 112 && (Event.current.alt || (int)Event.current.modifiers == 4) && Time.time - lastKeyToggle > 0.2f)
		{
			lastKeyToggle = Time.time;
			PlayerTagManager.Toggle();
		}
		AdminTagGUI.DrawAdminTagGUI();
		PlayerTagManager.DrawGUI();
		GUI.Label(new Rect((float)Screen.width / 2f - 75f, 10f, 150f, 25f), "Open GUI = " + (IsOpen ? "On" : "Off"));
		if (!IsOpen)
		{
			return;
		}
		if (Main.themeType != lastSyncedThemeType && !isRainbowTheme)
		{
			SyncFromVrTheme();
			lastSyncedThemeType = Main.themeType;
		}
		GUI.backgroundColor = guiBgColor;
		GUI.contentColor = guiContentColor;
		guiRect = GUI.Window(9999, guiRect, new WindowFunction(MainWindowFunction), "");
		if (showPlayers && selectedPlayerIndex >= 0 && selectedPlayerIndex < onlinePlayers.Count)
		{
			portraitWindowRect = new Rect(((Rect)(ref guiRect)).x + ((Rect)(ref guiRect)).width + 4f, ((Rect)(ref guiRect)).y, ((Rect)(ref portraitWindowRect)).width, ((Rect)(ref portraitWindowRect)).height);
			portraitWindowRect = GUI.Window(9998, portraitWindowRect, new WindowFunction(DrawPlayerPortraitWindow), "");
			cosmeticsWindowRect = new Rect(((Rect)(ref guiRect)).x - ((Rect)(ref cosmeticsWindowRect)).width - 4f, ((Rect)(ref guiRect)).y, ((Rect)(ref cosmeticsWindowRect)).width, ((Rect)(ref cosmeticsWindowRect)).height);
			cosmeticsWindowRect = GUI.Window(9997, cosmeticsWindowRect, new WindowFunction(DrawPlayerCosmeticsWindow), "");
		}
		if (showCosmetics)
		{
			mirrorWindowRect = new Rect(((Rect)(ref guiRect)).x + ((Rect)(ref guiRect)).width + 4f, ((Rect)(ref guiRect)).y, ((Rect)(ref mirrorWindowRect)).width, ((Rect)(ref mirrorWindowRect)).height);
			mirrorWindowRect = GUI.Window(9996, mirrorWindowRect, new WindowFunction(DrawMirrorWindow), "");
		}
		if (IsOpen && (int)Event.current.type == 7 && enableRainbowSnake)
		{
			if ((Object)(object)snakeDot == (Object)null)
			{
				snakeDot = new Texture2D(1, 1);
				snakeDot.SetPixel(0, 0, Color.white);
				snakeDot.Apply();
			}
			float num = 120f;
			snakeProgress += num * Time.deltaTime;
			float width = ((Rect)(ref guiRect)).width;
			float height = ((Rect)(ref guiRect)).height;
			float num2 = 2f * (width + height);
			float num3 = snakeProgress % num2;
			float num4;
			float num5;
			if (num3 < width)
			{
				num4 = num3;
				num5 = 0f;
			}
			else if (num3 < width + height)
			{
				num4 = width;
				num5 = num3 - width;
			}
			else if (num3 < 2f * width + height)
			{
				num4 = width - (num3 - width - height);
				num5 = height;
			}
			else
			{
				num4 = 0f;
				num5 = height - (num3 - 2f * width - height);
			}
			Vector2 position = default(Vector2);
			((Vector2)(ref position))._002Ector(((Rect)(ref guiRect)).x + num4, ((Rect)(ref guiRect)).y + num5);
			snakeTrail.Add(new TrailPoint
			{
				Position = position,
				Time = Time.realtimeSinceStartup,
				Hue = snakeProgress / num2 % 1f
			});
			if (snakeTrail.Count > 20)
			{
				snakeTrail.RemoveAt(0);
			}
			Color color = GUI.color;
			for (int i = 0; i < snakeTrail.Count; i++)
			{
				TrailPoint trailPoint = snakeTrail[i];
				float num6 = Time.realtimeSinceStartup - trailPoint.Time;
				float num7 = (float)i / (float)snakeTrail.Count;
				float num8 = 4f + 8f * num7;
				Color color2 = Color.HSVToRGB((trailPoint.Hue + num6 * 0.2f) % 1f, 1f, 1f);
				color2.a = 0.3f + 0.7f * num7;
				GUI.color = color2;
				GUI.DrawTexture(new Rect(trailPoint.Position.x - num8 / 2f, trailPoint.Position.y - num8 / 2f, num8, num8), (Texture)(object)snakeDot);
			}
			GUI.color = color;
		}
	}

	private void InitBlurGlow()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)blurGlow != (Object)null)
		{
			return;
		}
		int num = 256;
		blurGlow = new Texture2D(num, num, (TextureFormat)4, false);
		float num2 = (float)num / 2f;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float num3 = Vector2.Distance(new Vector2((float)i, (float)j), new Vector2(num2, num2)) / num2;
				float num4 = Mathf.Clamp01(1f - num3);
				num4 = num4 * num4 * 0.35f;
				blurGlow.SetPixel(i, j, new Color(1f, 1f, 1f, num4));
			}
		}
		blurGlow.Apply();
	}

	private void MainWindowFunction(int windowId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Expected O, but got Unknown
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Expected O, but got Unknown
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Expected O, but got Unknown
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 7 && enableMouseGlow)
		{
			InitBlurGlow();
			Vector2 mousePosition = Event.current.mousePosition;
			smoothMousePos = Vector2.Lerp(smoothMousePos, mousePosition, Time.deltaTime * 15f);
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (Vector2.Distance(mousePosition, (glowBlobs.Count > 0) ? glowBlobs[glowBlobs.Count - 1].Position : Vector2.zero) > 8f || glowBlobs.Count == 0)
			{
				glowBlobs.Add(new GlowBlob
				{
					Position = smoothMousePos,
					Time = realtimeSinceStartup,
					Hue = realtimeSinceStartup * 0.4f % 1f,
					Size = 100f + Random.Range(40f, 120f)
				});
				if (glowBlobs.Count > 50)
				{
					glowBlobs.RemoveAt(0);
				}
			}
			Color color = GUI.color;
			for (int i = 0; i < glowBlobs.Count; i++)
			{
				GlowBlob glowBlob = glowBlobs[i];
				float num = realtimeSinceStartup - glowBlob.Time;
				float num2 = 1.5f;
				float num3 = Mathf.Clamp01(1f - num / num2);
				if (!(num3 <= 0f))
				{
					float num4 = 1f + num * 0.8f;
					float num5 = glowBlob.Size * num4;
					Color color2 = Color.HSVToRGB((glowBlob.Hue + num * 0.15f) % 1f, 0.6f, 1f);
					color2.a = num3 * 0.25f;
					GUI.color = color2;
					GUI.DrawTexture(new Rect(glowBlob.Position.x - num5 / 2f, glowBlob.Position.y - num5 / 2f, num5, num5), (Texture)(object)blurGlow);
				}
			}
			GUI.color = color;
		}
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 25f));
		Color color3 = GUI.color;
		if (showWelcome)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 22,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4,
				richText = true
			};
			val.normal.textColor = guiColorA;
			GUI.Label(new Rect(0f, ((Rect)(ref guiRect)).height / 2f - 80f, ((Rect)(ref guiRect)).width, 30f), "Welcome to Seralyth Remake", val);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 13,
				alignment = (TextAnchor)4,
				wordWrap = true,
				richText = true
			};
			GUI.Label(new Rect(50f, ((Rect)(ref guiRect)).height / 2f - 40f, ((Rect)(ref guiRect)).width - 100f, 40f), "Click the Tour button to learn where everything is,\nor explore on your own!", val2);
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(((Rect)(ref guiRect)).width / 2f - 75f, ((Rect)(ref guiRect)).height / 2f + 20f, 150f, 30f), "Start Tour"))
			{
				showWelcome = false;
				PlayerPrefs.SetString("SeralythWelcomeVersion", "2026-09-29T13:10:37Z");
				PlayerPrefs.Save();
				tourIndex = 0;
				tourPrevIndex = -1;
				tourComplete = false;
				showTour = true;
				showMods = true;
				Rect val3 = tourTargets[0];
				tourFingerX = ((Rect)(ref val3)).x + ((Rect)(ref val3)).width * 0.5f;
				tourFingerY = ((Rect)(ref val3)).y + ((Rect)(ref val3)).height * 0.5f;
				tourFingerTargetX = tourFingerX;
				tourFingerTargetY = tourFingerY;
				tourFingerClickTime = 0f;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			if (GUI.Button(new Rect(((Rect)(ref guiRect)).width / 2f - 75f, ((Rect)(ref guiRect)).height / 2f + 60f, 150f, 30f), "Explore"))
			{
				showWelcome = false;
				PlayerPrefs.SetString("SeralythWelcomeVersion", "2026-09-29T13:10:37Z");
				PlayerPrefs.Save();
				showMods = true;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			GUI.color = guiIconColor;
			GUI.DrawTexture(new Rect(((Rect)(ref guiRect)).width / 2f - 25f, ((Rect)(ref guiRect)).height / 2f - 120f, 50f, 50f), (Texture)(object)GetMenuIcon());
			GUI.color = color3;
			return;
		}
		if (titleStyle == null)
		{
			titleStyle = new GUIStyle(GUI.skin.label);
			titleStyle.fontSize = 16;
			titleStyle.richText = true;
		}
		string text = ((!Main.disableCategoryDisplay && showMods && currentCategoryIndex >= 0) ? Buttons.categoryNames[currentCategoryIndex] : null);
		string text2 = ((text != null) ? (" <color=#" + ColorUtility.ToHtmlStringRGB(guiColorB) + ">[{catName}]</color>") : "");
		string text3 = (PhotonNetwork.IsConnected ? "<color=#00FF00>●</color>" : "<color=#FF0000>●</color>");
		GUIContent val4 = new GUIContent(text3 + " <color=#" + ColorUtility.ToHtmlStringRGB(guiContentColor) + ">Chicken</color> <color=#88888888>v10.0.2</color> <color=#" + ColorUtility.ToHtmlStringRGB(guiColorA) + ">FPS: " + Mathf.RoundToInt(1f / Time.deltaTime) + "</color>" + text2);
		Vector2 val5 = titleStyle.CalcSize(val4);
		GUI.Label(new Rect(165f, 2f, val5.x, 22f), val4, titleStyle);
		GUI.Label(new Rect(((Rect)(ref guiRect)).width - 160f, 2f, 155f, 22f), "Insert - Toggle GUI");
		GUI.color = guiIconColor;
		GUI.DrawTexture(new Rect(165f + val5.x + 6f, 0f, 24f, 24f), (Texture)(object)GetMenuIcon());
		GUI.DrawTexture(new Rect(((Rect)(ref guiRect)).width - 80f, ((Rect)(ref guiRect)).height / 2f - 25f, 50f, 50f), (Texture)(object)GetMenuIcon());
		GUI.color = color3;
		GUI.Box(new Rect(0f, 0f, 160f, 430f), "");
		bool enabled = GUI.enabled;
		if (showTour)
		{
			GUI.enabled = false;
		}
		GUI.backgroundColor = (showSearch ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(600f, 21f, 95f, 25f), "Search"))
		{
			showSearch = !showSearch;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (showSearch)
		{
			GUI.backgroundColor = guiColorB;
			searchText = GUI.TextField(new Rect(380f, 21f, 215f, 25f), searchText);
		}
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(600f, 50f, 95f, 25f), "Discord"))
		{
			Application.OpenURL("https://discord.gg/npJTZAH3cH");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		DrawSidebar();
		GUI.backgroundColor = guiColorB;
		if (showMods)
		{
			DrawModsTab();
		}
		else if (showPC)
		{
			DrawPCTab();
		}
		else if (showPlayers)
		{
			DrawPlayersTab();
		}
		else if (showPlayerColor)
		{
			DrawPlayerColorTab();
		}
		else if (showTheme)
		{
			DrawThemeTab();
		}
		else if (showCredits)
		{
			DrawCreditsTab();
		}
		else if (showAI)
		{
			DrawAITab();
		}
		else if (showSuggestions)
		{
			DrawSuggestionsTab();
		}
		else if (showShowcases)
		{
			DrawShowcasesTab();
		}
		else if (showCosmetics)
		{
			DrawCosmeticsTab();
		}
		else if (showIcon)
		{
			DrawIconTab();
		}
		else if (showChat)
		{
			DrawChatTab();
		}
		else if (showFriends)
		{
			DrawFriendsTab();
		}
		else if (showReview)
		{
			DrawReviewTab();
		}
		else if (showGUISettings)
		{
			DrawGUISettingsTab();
		}
		else if (showGames)
		{
			DrawGamesTab();
		}
		else if (showSupport)
		{
			DrawSupportTab();
		}
		if (!showCosmetics && (Object)(object)mirrorCamera != (Object)null && (Object)(object)VRRig.LocalRig != (Object)null && (Object)(object)VRRig.LocalRig.headMesh != (Object)null)
		{
			Transform transform = VRRig.LocalRig.headMesh.transform;
			((Component)mirrorCamera).transform.position = transform.position + transform.forward * 0.05f;
			((Component)mirrorCamera).transform.LookAt(transform.position);
			if (!((Component)mirrorCamera).gameObject.activeSelf)
			{
				((Component)mirrorCamera).gameObject.SetActive(true);
			}
		}
		if (showMods && !showTour)
		{
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(600f, 80f, 95f, 25f), "Tour"))
			{
				tourIndex = 0;
				tourPrevIndex = -1;
				tourComplete = false;
				showTour = true;
				showPC = false;
				showPlayers = false;
				showChat = false;
				showTheme = false;
				showCredits = false;
				showIcon = false;
				showShowcases = false;
				showCosmetics = false;
				selectedModCategory = -1;
				currentCategoryIndex = -1;
				Rect val6 = tourTargets[0];
				tourFingerX = ((Rect)(ref val6)).x + ((Rect)(ref val6)).width * 0.5f;
				tourFingerY = ((Rect)(ref val6)).y + ((Rect)(ref val6)).height * 0.5f;
				tourFingerTargetX = tourFingerX;
				tourFingerTargetY = tourFingerY;
				tourFingerClickTime = 0f;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
		}
		if (showTour)
		{
			GUI.enabled = true;
			DrawTourOverlay();
			GUI.enabled = false;
		}
		GUI.enabled = enabled;
		if (GUI.Button(new Rect(0f, 400f, 700f, 25f), "DISCONNECT"))
		{
			PhotonNetwork.Disconnect();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		DrawRoundedCorners(((Rect)(ref guiRect)).width, ((Rect)(ref guiRect)).height);
	}

	private void DrawSidebar()
	{
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		float num = 160f;
		float num2 = 374f;
		if (showMods)
		{
			BuildCategoryList();
			float num3 = (float)modCategoryNames.Length * 27f;
			modCategoryScrollPosition = GUI.BeginScrollView(new Rect(0f, 21f, num, num2), modCategoryScrollPosition, new Rect(0f, 0f, num - 5f, num3), false, true);
			for (int i = 0; i < modCategoryNames.Length; i++)
			{
				float num4 = (float)i * 27f;
				GUI.backgroundColor = ((selectedModCategory == i) ? guiColorA : guiColorB);
				if (GUI.Button(new Rect(5f, num4, num - 10f, 25f), modCategoryNames[i]))
				{
					selectedModCategory = i;
					currentCategoryIndex = -1;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
			GUI.EndScrollView();
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(5f, 390f, num - 10f, 25f), "Back to Tabs"))
			{
				showMods = false;
				selectedModCategory = -1;
				currentCategoryIndex = -1;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			return;
		}
		string[] array = new string[17]
		{
			"Mods", "PC", "Players", "Friends", "Chat", "Player Color", "Theme", "Credits", "Icon", "Showcases",
			"Cosmetics", "Suggestions", "AI", "Review", "GUI Settings", "Games", "Support"
		};
		string[] array2 = new string[17]
		{
			"Mods", "PC", "Players", "Friends", "Chat", "PlayerColor", "Theme", "Credits", "Icon", "Showcases",
			"Cosmetics", "Suggestions", "AI", "Review", "GUISettings", "Games", "Support"
		};
		float num5 = (float)array.Length * 27f;
		sidebarScrollPosition = GUI.BeginScrollView(new Rect(0f, 21f, num, num2), sidebarScrollPosition, new Rect(0f, 0f, num - 5f, num5), false, true);
		for (int j = 0; j < array.Length; j++)
		{
			float num6 = (float)j * 27f;
			GUI.backgroundColor = ((array2[j] == GetCurrentTab()) ? guiColorA : guiColorB);
			if (GUI.Button(new Rect(5f, num6, num - 10f, 25f), array[j]))
			{
				SelectTab(array2[j]);
			}
		}
		GUI.EndScrollView();
	}

	private bool DrawButton(float x, float y, float w, float h, string text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GUI.backgroundColor = guiColorB;
		return GUI.Button(new Rect(x, y, w, h), text);
	}

	public void SelectTab(string tab)
	{
		SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		showTour = tab == "Tour";
		showMods = tab == "Mods";
		showPC = tab == "PC";
		showPlayers = tab == "Players";
		showChat = tab == "Chat";
		showFriends = tab == "Friends";
		if (showFriends)
		{
			friendsNeedsRefresh = true;
		}
		showPlayerColor = tab == "PlayerColor";
		showTheme = tab == "Theme";
		showCredits = tab == "Credits";
		showIcon = tab == "Icon";
		showShowcases = tab == "Showcases";
		showCosmetics = tab == "Cosmetics";
		showSuggestions = tab == "Suggestions";
		showAI = tab == "AI";
		showReview = tab == "Review";
		showGUISettings = tab == "GUISettings";
		showGames = tab == "Games";
		showSupport = tab == "Support";
		BroadcastMenuStatus();
	}

	private string GetCurrentTab()
	{
		if (showMods)
		{
			return "Mods";
		}
		if (showPC)
		{
			return "PC";
		}
		if (showPlayers)
		{
			return "Players";
		}
		if (showPlayerColor)
		{
			return "PlayerColor";
		}
		if (showTheme)
		{
			return "Theme";
		}
		if (showCredits)
		{
			return "Credits";
		}
		if (showAI)
		{
			return "AI";
		}
		if (showReview)
		{
			return "Review";
		}
		if (showSuggestions)
		{
			return "Suggestions";
		}
		if (showShowcases)
		{
			return "Showcases";
		}
		if (showCosmetics)
		{
			return "Cosmetics";
		}
		if (showIcon)
		{
			return "Icon";
		}
		if (showChat)
		{
			return "Chat";
		}
		if (showFriends)
		{
			return "Friends";
		}
		if (showGUISettings)
		{
			return "GUISettings";
		}
		if (showGames)
		{
			return "Games";
		}
		if (showSupport)
		{
			return "Support";
		}
		return "Mods";
	}

	private void BroadcastMenuStatus()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		if (PhotonNetwork.InRoom)
		{
			string text = (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? "Unknown" : PhotonNetwork.LocalPlayer.NickName);
			PhotonNetwork.RaiseEvent((byte)83, (object)new object[4]
			{
				PhotonNetwork.LocalPlayer.ActorNumber,
				text,
				GetCurrentTab(),
				IsOpen
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
		}
	}

	private void DrawModsTab()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f59: Unknown result type (might be due to invalid IL or missing references)
		//IL_13df: Unknown result type (might be due to invalid IL or missing references)
		//IL_140a: Unknown result type (might be due to invalid IL or missing references)
		//IL_142f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1434: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f8: Expected O, but got Unknown
		//IL_161b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1648: Unknown result type (might be due to invalid IL or missing references)
		//IL_1009: Unknown result type (might be due to invalid IL or missing references)
		//IL_178f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1794: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a7: Expected O, but got Unknown
		//IL_17c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ca: Expected O, but got Unknown
		//IL_153b: Unknown result type (might be due to invalid IL or missing references)
		//IL_156c: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1903: Expected O, but got Unknown
		//IL_1907: Unknown result type (might be due to invalid IL or missing references)
		//IL_190c: Unknown result type (might be due to invalid IL or missing references)
		//IL_190e: Unknown result type (might be due to invalid IL or missing references)
		//IL_191d: Unknown result type (might be due to invalid IL or missing references)
		//IL_192e: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1591: Unknown result type (might be due to invalid IL or missing references)
		//IL_1596: Unknown result type (might be due to invalid IL or missing references)
		//IL_1966: Unknown result type (might be due to invalid IL or missing references)
		//IL_1954: Unknown result type (might be due to invalid IL or missing references)
		//IL_171d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1724: Expected O, but got Unknown
		//IL_1747: Unknown result type (might be due to invalid IL or missing references)
		//IL_1774: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_105a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053: Unknown result type (might be due to invalid IL or missing references)
		//IL_199c: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1981: Unknown result type (might be due to invalid IL or missing references)
		//IL_1988: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_106f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_126d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1273: Invalid comparison between Unknown and I4
		//IL_11ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1294: Unknown result type (might be due to invalid IL or missing references)
		//IL_1220: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1312: Unknown result type (might be due to invalid IL or missing references)
		//IL_139e: Unknown result type (might be due to invalid IL or missing references)
		int num = ((currentCategoryIndex >= 0) ? currentCategoryIndex : ((selectedModCategory >= 0) ? modCategoryIndices[selectedModCategory] : (-1)));
		if (num < 0)
		{
			GUI.Label(new Rect(170f, 50f, 300f, 25f), "Select a category from the sidebar");
			return;
		}
		GUI.Label(new Rect(170f, 50f, 300f, 25f), "<b>" + Buttons.categoryNames[num] + "</b>");
		List<ButtonInfo> list;
		if (!string.IsNullOrEmpty(searchText))
		{
			list = (from b in Buttons.buttons.SelectMany((ButtonInfo[] x) => x)
				where b.buttonText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
				select b).ToList();
		}
		else
		{
			switch (Buttons.categoryNames[num])
			{
			case "Favorite Mods":
				list = Main.StringsToInfos(Main.favorites.ToArray()).ToList();
				break;
			case "Enabled Mods":
				list = (from b in Buttons.buttons.SelectMany((ButtonInfo[] x) => x)
					where b.enabled && b.isTogglable
					select b).ToList();
				break;
			case "Quest Mods":
			{
				List<ButtonInfo> list3 = new List<ButtonInfo>();
				list3.Add(new ButtonInfo
				{
					buttonText = "Exit Quests",
					method = delegate
					{
						Buttons.CurrentCategoryName = "Main";
					},
					isTogglable = false,
					toolTip = "Returns you back to the main page.",
					legal = true
				});
				string arg2 = "green";
				list3.Add(new ButtonInfo
				{
					buttonText = $"Level: {Quests.playerLevel}",
					overlapText = $"Level <color=grey>[</color><color={arg2}>{Quests.playerLevel}</color><color=grey>]</color> ({2 - Quests.questsUntilNextLevel}/2)",
					isTogglable = false,
					toolTip = "Your quest level goes up every 2 completions. Infinite levels!",
					legal = true
				});
				string text9 = ((Quests.selectedDifficulty == "Random") ? "white" : ((Quests.selectedDifficulty == "Easy") ? "green" : ((Quests.selectedDifficulty == "Medium") ? "yellow" : "red")));
				list3.Add(new ButtonInfo
				{
					buttonText = "Change Quest Difficulty",
					overlapText = "Change Quest Difficulty <color=grey>[</color><color=" + text9 + ">" + Quests.selectedDifficulty + "</color><color=grey>]</color>",
					method = delegate
					{
						Quests.ChangeDifficulty();
					},
					enableMethod = delegate
					{
						Quests.ChangeDifficulty();
					},
					disableMethod = delegate
					{
						Quests.ChangeDifficulty(forward: false);
					},
					incremental = true,
					isTogglable = false,
					toolTip = "Changes the quest difficulty.",
					legal = true
				});
				if (Quests.activeQuestCheck != null)
				{
					list3.Add(new ButtonInfo
					{
						buttonText = "Current Quest",
						overlapText = Quests.activeDifficulty + " " + Quests.activeQuestName,
						isTogglable = false,
						toolTip = Quests.activeQuestDare,
						legal = true
					});
					list3.Add(new ButtonInfo
					{
						buttonText = "Get Hint",
						method = Quests.GiveHint,
						isTogglable = false,
						toolTip = "Get a hint about the current quest.",
						legal = true
					});
				}
				else
				{
					list3.Add(new ButtonInfo
					{
						buttonText = "Next Quest...",
						isTogglable = false,
						toolTip = $"New quest in {Mathf.Max(0, Mathf.CeilToInt(Quests.nextQuestTime - Time.time))}s",
						legal = true
					});
					if (Quests.lastCompletedName != null)
					{
						string text10 = (Quests.lastWasLevelUp ? $" <color=green>(LEVEL UP to {Quests.lastCompletedLevel + 1}!)</color>" : $" ({2 - Quests.questsUntilNextLevel}/2 to lvl {Quests.playerLevel + 1})");
						list3.Add(new ButtonInfo
						{
							buttonText = "Last: " + Quests.lastCompletedDifficulty + " " + Quests.lastCompletedName + text10,
							overlapText = "<color=green>QUEST COMPLETE</color> " + Quests.lastCompletedDifficulty + " <color=yellow>" + Quests.lastCompletedName + "</color>" + text10,
							isTogglable = false,
							toolTip = "The quest you just completed!",
							legal = true
						});
					}
				}
				list3.Add(new ButtonInfo
				{
					buttonText = $"Completed: {Quests.completedCount}",
					overlapText = $"Completed <color=grey>[</color><color=green>{Quests.completedCount}</color><color=grey>]</color>",
					isTogglable = false,
					toolTip = "Total quests completed.",
					legal = true
				});
				list3.Add(new ButtonInfo
				{
					buttonText = "Reset Quests",
					method = Quests.ResetAllQuests,
					isTogglable = false,
					toolTip = "Resets level, progress, and starts a new quest in 60 seconds.",
					legal = true
				});
				list = list3;
				break;
			}
			case "Macros":
			{
				List<ButtonInfo> list2 = new List<ButtonInfo>();
				list2.Add(new ButtonInfo
				{
					buttonText = "Exit Macros",
					method = delegate
					{
						Buttons.CurrentCategoryName = "Movement Mods";
					},
					isTogglable = false,
					toolTip = "Returns you back to the movement mods.",
					legal = true
				});
				list2.Add(new ButtonInfo
				{
					buttonText = "Record <color=grey>[</color><color=green>T</color><color=grey>]</color>",
					method = Movement.RecordMacro,
					toolTip = "Record your macros with your <color=green>left trigger</color>."
				});
				list2.Add(new ButtonInfo
				{
					buttonText = "Macro Gun",
					method = Movement.MacroGun,
					toolTip = "Record your macros using a <color=green>gun</color>. Grip to aim, trigger to record."
				});
				list2.Add(new ButtonInfo
				{
					buttonText = "Reload Macros",
					method = Movement.LoadMacros,
					isTogglable = false,
					toolTip = "Reloads your macros."
				});
				if (isRecordingPlayerMacro)
				{
					string text = Mathf.RoundToInt(recordingPlayerColor.r * 255f).ToString("X2");
					string text2 = Mathf.RoundToInt(recordingPlayerColor.g * 255f).ToString("X2");
					string text3 = Mathf.RoundToInt(recordingPlayerColor.b * 255f).ToString("X2");
					string arg = text + text2 + text3;
					list2.Add(new ButtonInfo
					{
						buttonText = "Recording for " + recordingPlayerName + "...",
						overlapText = $"<color=#{arg}>Recording [{recordingPlayerName}]</color> ({currentRecordingSteps.Count} steps)",
						isTogglable = false,
						toolTip = "Currently recording a macro. Press Stop Recording to finish.",
						legal = true
					});
					list2.Add(new ButtonInfo
					{
						buttonText = "Stop Recording",
						isTogglable = false,
						method = StopPlayerMacroRecording,
						toolTip = "Stops recording and saves the macro.",
						legal = true
					});
				}
				else if (isPlayingPlayerMacro)
				{
					list2.Add(new ButtonInfo
					{
						buttonText = "Playing macro for " + playingMacroPlayerName + "...",
						overlapText = $"<color=yellow>Playing [{playingMacroPlayerName}]</color> ({macroPlayIndex}/{GetStoredMacroCount(playingMacroPlayerName)})",
						isTogglable = false,
						toolTip = "Currently playing a macro.",
						legal = true
					});
					list2.Add(new ButtonInfo
					{
						buttonText = "Stop Playback",
						isTogglable = false,
						method = StopPlayerMacroPlayback,
						toolTip = "Stops the macro playback.",
						legal = true
					});
				}
				IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
				if (activeRigs != null)
				{
					for (int num2 = 0; num2 < activeRigs.Count; num2++)
					{
						VRRig val = activeRigs[num2];
						if ((Object)(object)val == (Object)null || val.isLocal)
						{
							continue;
						}
						string name = val.GetName();
						if (string.IsNullOrEmpty(name) || name == "null")
						{
							continue;
						}
						Color val2 = (((Object)(object)val.mainSkin != (Object)null) ? ((Renderer)val.mainSkin).material.color : val.playerColor);
						string text4 = Mathf.RoundToInt(val2.r * 255f).ToString("X2");
						string text5 = Mathf.RoundToInt(val2.g * 255f).ToString("X2");
						string text6 = Mathf.RoundToInt(val2.b * 255f).ToString("X2");
						string text7 = text4 + text5 + text6;
						int capturedR = num2;
						bool flag = playerMacroStore.ContainsKey(name) && playerMacroStore[name].Count > 0;
						if (isRecordingPlayerMacro && recordingPlayerName == name)
						{
							list2.Add(new ButtonInfo
							{
								buttonText = "Record " + name + " Macro",
								overlapText = "<color=#" + text7 + ">Record " + name + " Macro</color> <color=red>[REC]</color>",
								isTogglable = false,
								toolTip = "Currently recording for " + name + ".",
								legal = true
							});
						}
						else
						{
							string text8 = (flag ? $" <color=green>[{playerMacroStore[name].Count}]</color>" : "");
							list2.Add(new ButtonInfo
							{
								buttonText = "Record " + name + " Macro",
								overlapText = "<color=#" + text7 + ">Record " + name + " Macro</color>" + text8,
								isTogglable = false,
								method = delegate
								{
									StartPlayerMacroRecording(capturedR);
								},
								toolTip = (flag ? $"Record a new macro for {name}. Right-click to play existing ({playerMacroStore[name].Count} steps)." : ("Record a macro for " + name + ".")),
								legal = true
							});
						}
						if (flag && !isRecordingPlayerMacro && !isPlayingPlayerMacro)
						{
							list2.Add(new ButtonInfo
							{
								buttonText = "Play " + name + " Macro",
								overlapText = "<color=#" + text7 + ">Play " + name + " Macro</color>",
								isTogglable = false,
								method = delegate
								{
									StartPlayerMacroPlayback(capturedR);
								},
								toolTip = "Play the recorded macro for " + name + ".",
								legal = true
							});
						}
					}
				}
				if (playerMacroStore.Count > 0 && !isRecordingPlayerMacro && !isPlayingPlayerMacro)
				{
					list2.Add(new ButtonInfo
					{
						buttonText = "Clear All Macros",
						isTogglable = false,
						method = ClearAllPlayerMacros,
						toolTip = "Deletes all recorded player macros.",
						legal = true
					});
				}
				list = list2;
				break;
			}
			case "Achievements":
				AchievementManager.EnterAchievementTab();
				list = Buttons.buttons[num].ToList();
				break;
			case "Friends":
				if (friendsNeedsRefresh)
				{
					friendsNeedsRefresh = false;
					FriendManager.FriendsListUpdated();
				}
				list = Buttons.buttons[num].ToList();
				break;
			default:
				list = Buttons.buttons[num].ToList();
				break;
			}
		}
		float num3 = 25f;
		float num4 = 75f;
		float num5 = 340f;
		int count = list.Count;
		int num6 = 12;
		int num7 = Mathf.Max(1, Mathf.CeilToInt((float)count / (float)num6));
		bool flag2 = !Main.pageScrolling && num7 > 1;
		float num8 = (flag2 ? (((Rect)(ref guiRect)).height - num4 - 85f) : (((Rect)(ref guiRect)).height - num4 - 55f));
		if (!Main.pageScrolling)
		{
			pcPageNumber = Mathf.Clamp(pcPageNumber, 0, num7 - 1);
			list = list.Skip(pcPageNumber * num6).Take(num6).ToList();
		}
		hoveredTooltip = "";
		float num9 = (float)list.Count * (num3 + buttonSpacingY) + 20f;
		modScrollPosition = GUI.BeginScrollView(new Rect(170f, num4, ((Rect)(ref guiRect)).width - 180f, num8), modScrollPosition, new Rect(0f, 0f, num5, num9), false, true);
		int num10 = 0;
		Rect val3 = default(Rect);
		for (int num11 = 0; num11 < list.Count; num11++)
		{
			if (list[num11].buttonText == "configuration")
			{
				continue;
			}
			float num12 = (float)num10 * (num3 + buttonSpacingY);
			bool flag3 = Main.favorites.Contains(list[num11].buttonText);
			string text11 = (string.IsNullOrEmpty(list[num11].overlapText) ? list[num11].buttonText : list[num11].overlapText);
			if (list[num11].label)
			{
				GUI.Label(new Rect(0f, num12, num5 - 40f, num3), text11);
				num10++;
				continue;
			}
			float num13 = (list[num11].incremental ? (num5 - 110f) : (num5 - 40f));
			GUI.backgroundColor = (list[num11].enabled ? guiColorA : guiColorB);
			if (GUI.Button(new Rect(0f, num12, num13, num3), text11))
			{
				if (list[num11].buttonText.StartsWith("Exit "))
				{
					string categoryName = list[num11].buttonText.Substring(5);
					int category = Buttons.GetCategory(categoryName);
					if (category >= 0)
					{
						int num14 = Array.IndexOf(modCategoryIndices, category);
						if (num14 >= 0)
						{
							selectedModCategory = num14;
							currentCategoryIndex = -1;
						}
						else
						{
							currentCategoryIndex = category;
						}
					}
				}
				else
				{
					int category2 = Buttons.GetCategory(list[num11].buttonText);
					if (category2 >= 0)
					{
						currentCategoryIndex = category2;
					}
					else if (list[num11].incremental)
					{
						Main.ToggleIncremental(list[num11].buttonText, increment: true);
					}
					else
					{
						Main.Toggle(list[num11]);
					}
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			if (list[num11].incremental)
			{
				GUI.backgroundColor = guiColorB;
				if (GUI.Button(new Rect(num5 - 110f, num12, 35f, num3), "-"))
				{
					Main.ToggleIncremental(list[num11].buttonText, increment: false);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				if (GUI.Button(new Rect(num5 - 75f, num12, 35f, num3), "+"))
				{
					Main.ToggleIncremental(list[num11].buttonText, increment: true);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
			if ((int)Event.current.type == 7)
			{
				((Rect)(ref val3))._002Ector(0f, num12, num13, num3);
				if (((Rect)(ref val3)).Contains(Event.current.mousePosition))
				{
					hoveredTooltip = list[num11].toolTip;
					hoveredButtonRect = new Rect(170f, num4 + num12 - modScrollPosition.y, num13, num3);
					hoveredButtonOriginalLabel = text11;
				}
			}
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = (flag3 ? guiColorA : guiColorB);
			if (GUI.Button(new Rect(num5 - 35f, num12, 35f, num3), flag3 ? "★" : "☆"))
			{
				if (Main.favorites.Contains(list[num11].buttonText))
				{
					Main.favorites.Remove(list[num11].buttonText);
				}
				else
				{
					Main.favorites.Add(list[num11].buttonText);
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = backgroundColor;
			num10++;
		}
		GUI.EndScrollView();
		if (flag2)
		{
			float num15 = 370f;
			GUI.backgroundColor = guiColorB;
			GUI.enabled = pcPageNumber > 0;
			if (GUI.Button(new Rect(180f, num15, 50f, 25f), "< Prev"))
			{
				pcPageNumber--;
				modScrollPosition = Vector2.zero;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.enabled = true;
			int num16 = Mathf.Min(num7, 10);
			int num17 = num16 / 2;
			int num18 = Mathf.Clamp(pcPageNumber - num17, 0, Mathf.Max(0, num7 - num16));
			float num19 = 235f;
			for (int num20 = 0; num20 < num16; num20++)
			{
				int num21 = num18 + num20;
				GUI.backgroundColor = ((num21 == pcPageNumber) ? guiColorA : guiColorB);
				int num22 = num21;
				if (GUI.Button(new Rect(num19, num15, 25f, 25f), (num22 + 1).ToString()))
				{
					pcPageNumber = num22;
					modScrollPosition = Vector2.zero;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				num19 += 27f;
			}
			GUI.backgroundColor = guiColorB;
			GUI.enabled = pcPageNumber < num7 - 1;
			if (GUI.Button(new Rect(num19 + 2f, num15, 50f, 25f), "Next >"))
			{
				pcPageNumber++;
				modScrollPosition = Vector2.zero;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.enabled = true;
		}
		if (string.IsNullOrEmpty(hoveredTooltip))
		{
			return;
		}
		if (tooltipStyle == 3)
		{
			GUIStyle val4 = new GUIStyle(GUI.skin.button);
			val4.alignment = (TextAnchor)4;
			val4.fontSize = 12;
			val4.wordWrap = false;
			val4.normal.textColor = Color.white;
			val4.normal.background = GUI.skin.box.normal.background;
			GUI.Label(hoveredButtonRect, hoveredTooltip, val4);
			return;
		}
		if (tooltipStyle == 4)
		{
			if (hoveredTooltip != typewriterTarget)
			{
				typewriterTarget = hoveredTooltip;
				typewriterChars = 0;
				typewriterTimer = Time.realtimeSinceStartup;
			}
			float num23 = 30f;
			typewriterChars = Mathf.Min((int)((Time.realtimeSinceStartup - typewriterTimer) * num23), typewriterTarget.Length);
			string text12 = typewriterTarget.Substring(0, typewriterChars);
			if (typewriterChars < typewriterTarget.Length)
			{
				text12 += "█";
			}
			GUIStyle val5 = new GUIStyle(GUI.skin.button);
			val5.alignment = (TextAnchor)4;
			val5.fontSize = 12;
			val5.wordWrap = false;
			val5.normal.textColor = Color.white;
			val5.normal.background = GUI.skin.box.normal.background;
			GUI.Label(hoveredButtonRect, text12, val5);
			return;
		}
		Vector2 mousePosition = Event.current.mousePosition;
		GUIStyle val6 = new GUIStyle(GUI.skin.box);
		val6.alignment = (TextAnchor)3;
		val6.fontSize = 11;
		val6.padding = new RectOffset(6, 6, 4, 4);
		val6.wordWrap = false;
		string text13 = hoveredTooltip;
		float num24 = 1f;
		if (tooltipStyle != 0)
		{
			if (hoveredTooltip != typewriterTarget)
			{
				typewriterTarget = hoveredTooltip;
				typewriterChars = 0;
				typewriterTimer = Time.realtimeSinceStartup;
			}
			if (tooltipStyle == 1)
			{
				float num25 = 30f;
				typewriterChars = Mathf.Min((int)((Time.realtimeSinceStartup - typewriterTimer) * num25), typewriterTarget.Length);
				text13 = typewriterTarget.Substring(0, typewriterChars);
				if (typewriterChars < typewriterTarget.Length)
				{
					text13 += "█";
				}
			}
			else if (tooltipStyle == 2)
			{
				float num26 = 0.4f;
				num24 = Mathf.Clamp01((Time.realtimeSinceStartup - typewriterTimer) / num26);
			}
		}
		val6.normal.textColor = new Color(1f, 1f, 1f, num24);
		GUIContent val7 = new GUIContent(text13);
		Vector2 val8 = val6.CalcSize(val7);
		float num27 = mousePosition.x + 15f;
		float num28 = mousePosition.y + 15f;
		if (num27 + val8.x > ((Rect)(ref guiRect)).width)
		{
			num27 = ((Rect)(ref guiRect)).width - val8.x - 5f;
		}
		if (num28 + val8.y > ((Rect)(ref guiRect)).height)
		{
			num28 = mousePosition.y - val8.y - 5f;
		}
		GUI.Box(new Rect(num27, num28, val8.x, val8.y), val7, val6);
	}

	private void DrawPCTab()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		guiScrollPosition = GUI.BeginScrollView(new Rect(170f, 21f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), guiScrollPosition, new Rect(0f, 0f, 500f, 360f), false, true);
		roomInput = GUI.TextField(new Rect(0f, 0f, 300f, 30f), roomInput);
		if (GUI.Button(new Rect(0f, 34f, 300f, 30f), "Set Name"))
		{
			PhotonNetwork.LocalPlayer.NickName = roomInput;
			PhotonNetwork.NickName = roomInput;
			PlayerPrefs.SetString("GTPlayerName", roomInput);
			((GorillaComputer)GorillaComputer.instance).currentName = roomInput;
			((Object)GorillaComputer.instance).name = roomInput;
			PlayerPrefs.Save();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(0f, 68f, 300f, 30f), "Join Room"))
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(roomInput, (JoinType)0);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.Label(new Rect(0f, 102f, 300f, 30f), "WASD Speed = " + wasdSpeed.ToString("F2"));
		wasdSpeed = GUI.HorizontalSlider(new Rect(0f, 124f, 300f, 30f), wasdSpeed, 0f, 10f);
		GUI.Label(new Rect(0f, 154f, 300f, 30f), "WASD Rotation = " + wasdRotation.ToString("F2"));
		wasdRotation = GUI.HorizontalSlider(new Rect(0f, 176f, 300f, 30f), wasdRotation, 0f, 10f);
		GUI.Label(new Rect(0f, 206f, 300f, 30f), "WASD Jump = " + wasdJump.ToString("F2"));
		wasdJump = GUI.HorizontalSlider(new Rect(0f, 228f, 300f, 30f), wasdJump, 0f, 10f);
		GUI.backgroundColor = (wasdEnabled ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(0f, 259f, 300f, 30f), "WASD Fly"))
		{
			wasdEnabled = !wasdEnabled;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.EndScrollView();
	}

	private void DrawTourOverlay()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0948: Unknown result type (might be due to invalid IL or missing references)
		//IL_0968: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Expected O, but got Unknown
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Expected O, but got Unknown
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd3: Unknown result type (might be due to invalid IL or missing references)
		int[] array = new int[24]
		{
			-1, -1, -1, 0, 1, 2, 3, -1, 4, 5,
			6, 7, 8, 9, 10, 11, -1, -1, -1, -1,
			-1, 12, 13, 14
		};
		int num = ((tourIndex >= 0 && tourIndex < array.Length) ? array[tourIndex] : (-1));
		Rect t = default(Rect);
		if (num >= 0)
		{
			((Rect)(ref t))._002Ector(5f, 21f + (float)num * 27f - sidebarScrollPosition.y, 150f, 25f);
		}
		else
		{
			t = tourTargets[tourIndex];
		}
		float overlayW = 320f;
		float overlayH = 190f;
		float maxOY;
		if (tourPrevIndex != tourIndex)
		{
			if (tourIndex >= 0 && tourIndex < array.Length)
			{
				int num2 = array[tourIndex];
				if (num2 >= 0)
				{
					float num3 = (float)num2 * 27f;
					float num4 = 374f;
					if (num3 < sidebarScrollPosition.y)
					{
						sidebarScrollPosition.y = num3;
					}
					else if (num3 + 25f > sidebarScrollPosition.y + num4)
					{
						sidebarScrollPosition.y = num3 + 25f - num4;
					}
				}
			}
			maxOY = 355f - overlayH;
			if (tourPrevIndex < 0)
			{
				tourAnimRect = t;
				tourOverlayX = CalcOx(t);
				tourOverlayY = CalcOy(t);
				tourOverlayTargetX = tourOverlayX;
				tourOverlayTargetY = tourOverlayY;
			}
			else
			{
				tourAnimRect = new Rect(((Rect)(ref tourAnimRect)).x, ((Rect)(ref tourAnimRect)).y, ((Rect)(ref tourAnimRect)).width, ((Rect)(ref tourAnimRect)).height);
				tourOverlayTargetX = CalcOx(t);
				tourOverlayTargetY = CalcOy(t);
			}
			tourFingerTargetX = ((Rect)(ref t)).x + ((Rect)(ref t)).width * 0.5f;
			tourFingerTargetY = ((Rect)(ref t)).y + ((Rect)(ref t)).height * 0.5f;
			tourFingerClickTime = 0f;
			tourAnimTime = 0f;
			tourPrevIndex = tourIndex;
		}
		tourAnimTime += Time.deltaTime;
		float num5 = Mathf.Clamp01(tourAnimTime / 0.7f);
		float num6 = num5 * num5 * (3f - 2f * num5);
		Rect target = default(Rect);
		((Rect)(ref target))._002Ector(Mathf.Lerp(((Rect)(ref tourAnimRect)).x, ((Rect)(ref t)).x, num6), Mathf.Lerp(((Rect)(ref tourAnimRect)).y, ((Rect)(ref t)).y, num6), Mathf.Lerp(((Rect)(ref tourAnimRect)).width, ((Rect)(ref t)).width, num6), Mathf.Lerp(((Rect)(ref tourAnimRect)).height, ((Rect)(ref t)).height, num6));
		tourAnimRect = target;
		float num7 = Mathf.Lerp(tourOverlayX, tourOverlayTargetX, num6);
		float num8 = Mathf.Lerp(tourOverlayY, tourOverlayTargetY, num6);
		tourOverlayX = num7;
		tourOverlayY = num8;
		bool flag = tourIndex == 0 || tourIndex == 1 || tourIndex == 2;
		bool flag2 = tourIndex == 6 || tourIndex == 7;
		bool flag3 = tourIndex == 15;
		bool flag4 = tourIndex == 20;
		bool flag5 = tourIndex == 22;
		if (flag4 && prevCategoryStep != tourIndex)
		{
			showMods = true;
			showPC = false;
			showPlayers = false;
			showChat = false;
			showPlayerColor = false;
			showTheme = false;
			showCredits = false;
			showIcon = false;
			showShowcases = false;
			showSuggestions = false;
			showAI = false;
			showGUISettings = false;
			pcPageNumber = 0;
			Main.pageScrolling = false;
			modScrollPosition = Vector2.zero;
			int num9 = ((Buttons.categoryNames != null) ? Array.IndexOf(Buttons.categoryNames, "Fun") : (-1));
			if (num9 >= 0)
			{
				int num10 = Array.IndexOf(modCategoryIndices, num9);
				if (num10 >= 0)
				{
					selectedModCategory = num10;
					currentCategoryIndex = -1;
				}
				else
				{
					currentCategoryIndex = num9;
					selectedModCategory = -1;
				}
			}
		}
		else if (flag && prevCategoryStep != tourIndex)
		{
			showMods = true;
			showPC = false;
			showPlayers = false;
			showChat = false;
			showPlayerColor = false;
			showTheme = false;
			showCredits = false;
			showIcon = false;
			showShowcases = false;
			showSuggestions = false;
			showAI = false;
			showGUISettings = false;
			selectedModCategory = -1;
			currentCategoryIndex = -1;
		}
		else if (flag3 && prevCategoryStep != tourIndex)
		{
			showMods = false;
			showPC = false;
			showPlayers = false;
			showChat = false;
			showPlayerColor = false;
			showTheme = false;
			showCredits = false;
			showIcon = false;
			showShowcases = false;
			showSuggestions = false;
			showAI = true;
			showGUISettings = false;
			selectedModCategory = -1;
			currentCategoryIndex = -1;
		}
		else if (flag2 && prevCategoryStep != tourIndex)
		{
			showMods = false;
			showPC = false;
			showPlayers = false;
			showChat = true;
			showPlayerColor = false;
			showTheme = false;
			showCredits = false;
			showIcon = false;
			showShowcases = false;
			showSuggestions = false;
			showAI = false;
			showGUISettings = false;
			selectedModCategory = -1;
			currentCategoryIndex = -1;
		}
		else if (flag5 && prevCategoryStep != tourIndex)
		{
			showMods = false;
			showPC = false;
			showPlayers = false;
			showChat = false;
			showPlayerColor = false;
			showTheme = false;
			showCredits = false;
			showIcon = false;
			showShowcases = false;
			showSuggestions = false;
			showAI = false;
			showGUISettings = true;
			selectedModCategory = -1;
			currentCategoryIndex = -1;
		}
		else if (!flag && !flag2 && !flag3 && !flag5 && prevCategoryStep >= 0)
		{
			showMods = false;
			showChat = false;
			showAI = false;
			showGUISettings = false;
		}
		prevCategoryStep = ((flag || flag2 || flag3 || flag4 || flag5) ? tourIndex : (-1));
		Color color = GUI.color;
		GUI.Box(new Rect(num7, num8, overlayW, overlayH), "");
		GUI.backgroundColor = guiColorA;
		GUI.Box(new Rect(num7 + 10f, num8 + 10f, overlayW - 20f, 25f), "Tour Guide");
		GUI.backgroundColor = guiColorB;
		if (tourComplete)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4,
				richText = true
			};
			val.normal.textColor = guiColorA;
			GUI.Label(new Rect(num7 + 15f, num8 + 60f, overlayW - 30f, 50f), "You're all set!", val);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				alignment = (TextAnchor)4,
				wordWrap = true
			};
			GUI.Label(new Rect(num7 + 15f, num8 + 100f, overlayW - 30f, 40f), "You now know your way around Seralyth Remake. Enjoy!", val2);
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(num7 + 20f, num8 + overlayH - 32f, overlayW - 40f, 22f), "Done"))
			{
				showTour = false;
				tourComplete = false;
				showMods = true;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			DrawTourHighlight(target);
			GUI.color = color;
			return;
		}
		float num11 = 60f;
		tourScrollPosition = GUI.BeginScrollView(new Rect(num7 + 15f, num8 + 45f, overlayW - 30f, num11), tourScrollPosition, new Rect(0f, 0f, overlayW - 50f, 100f), false, true);
		GUI.Label(new Rect(0f, 0f, overlayW - 50f, 100f), tourSteps[tourIndex]);
		GUI.EndScrollView();
		string text = "Step " + (tourIndex + 1) + " of " + tourSteps.Length;
		GUI.Label(new Rect(num7 + 20f, num8 + 110f, overlayW - 40f, 20f), text);
		GUI.enabled = tourIndex > 0;
		if (GUI.Button(new Rect(num7 + 20f, num8 + 135f, 130f, 25f), "< Previous"))
		{
			tourIndex = (tourIndex - 1 + tourSteps.Length) % tourSteps.Length;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.enabled = true;
		if (GUI.Button(new Rect(num7 + overlayW - 150f, num8 + 135f, 130f, 25f), (tourIndex >= tourSteps.Length - 1) ? "Finish" : "Next >"))
		{
			if (tourIndex >= tourSteps.Length - 1)
			{
				tourComplete = true;
			}
			else
			{
				tourIndex++;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = Color.red;
		if (GUI.Button(new Rect(num7 + overlayW - 140f, num8 + overlayH - 32f, 120f, 22f), "Done"))
		{
			showTour = false;
			showMods = false;
			selectedModCategory = -1;
			currentCategoryIndex = -1;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		DrawTourHighlight(target);
		DrawTourFinger();
		GUI.color = color;
		float CalcOx(Rect val3)
		{
			if (((Rect)(ref val3)).x < 160f)
			{
				return 165f;
			}
			return Mathf.Clamp(((Rect)(ref val3)).x + ((Rect)(ref val3)).width + 10f, 0f, 700f - overlayW);
		}
		float CalcOy(Rect val3)
		{
			if (((Rect)(ref val3)).x < 160f)
			{
				return Mathf.Clamp(((Rect)(ref val3)).y - 20f, 0f, maxOY);
			}
			if (((Rect)(ref val3)).y + ((Rect)(ref val3)).height + overlayH > 380f)
			{
				return Mathf.Clamp(((Rect)(ref val3)).y - overlayH - 10f, 0f, maxOY);
			}
			return Mathf.Clamp(((Rect)(ref val3)).y + ((Rect)(ref val3)).height + 10f, 0f, maxOY);
		}
	}

	private void DrawTourHighlight(Rect target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = Color.yellow;
		GUI.Box(new Rect(((Rect)(ref target)).x, ((Rect)(ref target)).y, ((Rect)(ref target)).width, ((Rect)(ref target)).height), "");
		GUI.backgroundColor = backgroundColor;
	}

	private void DrawTourFinger()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)tourCursorTex == (Object)null)
		{
			tourCursorTex = new Texture2D(1, 1);
			tourCursorTex.SetPixel(0, 0, Color.white);
			tourCursorTex.Apply();
		}
		tourFingerX = Mathf.Lerp(tourFingerX, tourFingerTargetX, Time.deltaTime * 5f);
		tourFingerY = Mathf.Lerp(tourFingerY, tourFingerTargetY, Time.deltaTime * 5f);
		bool flag = Mathf.Abs(tourFingerX - tourFingerTargetX) < 1f && Mathf.Abs(tourFingerY - tourFingerTargetY) < 1f;
		if (flag)
		{
			tourFingerX = tourFingerTargetX;
			tourFingerY = tourFingerTargetY;
		}
		tourFingerClickTime += Time.deltaTime;
		float num = 0f;
		if (flag)
		{
			float num2 = 0.6f;
			float num3 = tourFingerClickTime % num2;
			if (num3 > 0.35f && num3 < 0.45f)
			{
				num = Mathf.Lerp(0f, 6f, (num3 - 0.35f) / 0.1f);
			}
			else if (num3 >= 0.45f && num3 < 0.55f)
			{
				num = Mathf.Lerp(6f, 0f, (num3 - 0.45f) / 0.1f);
			}
			else if (num3 >= 0.55f)
			{
				num = 0f;
			}
		}
		float num4 = tourFingerX;
		float num5 = tourFingerY - 30f + num;
		Color color = GUI.color;
		GUI.color = Color.yellow;
		GUI.DrawTexture(new Rect(num4 - 2f, num5, 8f, 28f), (Texture)(object)tourCursorTex);
		GUI.DrawTexture(new Rect(num4 - 8f, num5, 14f, 8f), (Texture)(object)tourCursorTex);
		GUI.DrawTexture(new Rect(num4 - 10f, num5 + 4f, 8f, 6f), (Texture)(object)tourCursorTex);
		GUI.DrawTexture(new Rect(num4 + 2f, num5 + 4f, 12f, 6f), (Texture)(object)tourCursorTex);
		GUI.DrawTexture(new Rect(num4 + 2f, num5 + 8f, 16f, 6f), (Texture)(object)tourCursorTex);
		GUI.DrawTexture(new Rect(num4 - 10f, num5 + 8f, 10f, 6f), (Texture)(object)tourCursorTex);
		GUI.DrawTexture(new Rect(num4 - 6f, num5 + 12f, 16f, 6f), (Texture)(object)tourCursorTex);
		GUI.DrawTexture(new Rect(num4 - 4f, num5 + 18f, 12f, 10f), (Texture)(object)tourCursorTex);
		if (flag)
		{
			Rect val = tourAnimRect;
			float num6 = ((Rect)(ref val)).x + ((Rect)(ref val)).width * 0.5f;
			float num7 = ((Rect)(ref val)).y + ((Rect)(ref val)).height * 0.5f;
			float num8 = Mathf.Max(((Rect)(ref val)).width, ((Rect)(ref val)).height) * 0.6f;
			float num9 = tourFingerClickTime * 2f;
			int num10 = 24;
			float num11 = 4.3982296f;
			float num12 = 3f;
			for (int i = 0; i < num10; i++)
			{
				float num13 = num9 + (float)i / (float)num10 * num11;
				float num14 = num9 + (float)(i + 1) / (float)num10 * num11;
				float num15 = 1f - (float)i / (float)num10 * 0.7f;
				GUI.color = new Color(1f, 1f, 0f, num15 * 0.9f);
				float num16 = num6 + Mathf.Cos(num13) * num8;
				float num17 = num7 + Mathf.Sin(num13) * num8;
				float num18 = num6 + Mathf.Cos(num14) * num8;
				float num19 = num7 + Mathf.Sin(num14) * num8;
				float num20 = num18 - num16;
				float num21 = num19 - num17;
				float num22 = Mathf.Sqrt(num20 * num20 + num21 * num21);
				if (!(num22 < 0.1f))
				{
					float num23 = Mathf.Min(num16, num18);
					float num24 = Mathf.Min(num17, num19);
					GUI.DrawTexture(new Rect(num23, num24, Mathf.Max(num22, num12), Mathf.Max(num22, num12)), (Texture)(object)tourCursorTex);
				}
			}
		}
		GUI.color = color;
	}

	private void OnSuggestionEvent(EventData data)
	{
		if (data.Code == 82 && data.CustomData is object[] array && array.Length >= 4)
		{
			string text = array[0] as string;
			string text2 = array[1] as string;
			string text3 = array[2] as string;
			string photoUrl = array[3] as string;
			int actor = (int)array[4];
			if (text != null && text2 != null && text3 != null)
			{
				suggestionList.Add(new SuggestionEntry
				{
					sender = text,
					title = text2,
					message = text3,
					photoUrl = photoUrl,
					actor = actor
				});
				SaveSuggestions();
			}
		}
	}

	private void OnMenuStatusEvent(EventData data)
	{
		if (data.Code != 83 || !(data.CustomData is object[] array) || array.Length < 4)
		{
			return;
		}
		int actor = (int)array[0];
		if (actor == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		string text = array[1] as string;
		string text2 = array[2] as string;
		bool isOpen = (bool)array[3];
		if (text != null && text2 != null)
		{
			menuStatusList.RemoveAll((MenuStatusEntry e) => e.actor == actor);
			menuStatusList.Add(new MenuStatusEntry
			{
				actor = actor,
				nickname = text,
				tab = text2,
				isOpen = isOpen
			});
		}
	}

	private void OnChatEvent(EventData data)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (data.Code != 80 || !(data.CustomData is object[] array) || array.Length < 3)
		{
			return;
		}
		string text = array[0] as string;
		string text2 = array[1] as string;
		int num = (int)array[2];
		if (text != null && text2 != null)
		{
			string text3 = ((num == PhotonNetwork.LocalPlayer.ActorNumber) ? " (you)" : "");
			chatMessages.Add("<color=#" + ColorUtility.ToHtmlStringRGB(guiColorA) + ">" + text + "</color>" + text3 + ": " + text2);
			if (chatMessages.Count > 100)
			{
				chatMessages.RemoveAt(0);
			}
		}
	}

	private void OnAnnounceEvent(EventData data)
	{
		if (data.Code == 81)
		{
			if (data.CustomData is object[] array && array.Length >= 4)
			{
				string text = array[0] as string;
				string text2 = array[1] as string;
				int actor = (int)array[2];
				long id = (long)array[3];
				if (text != null && text2 != null)
				{
					AddAnnounceLocal(text, text2, actor, id);
				}
			}
		}
		else
		{
			if (data.Code != 82 || !(data.CustomData is object[] array2) || array2.Length < 1)
			{
				return;
			}
			long num = (long)array2[0];
			bool flag = false;
			for (int i = 0; i < announceData.Count; i++)
			{
				if (announceData[i].id == num)
				{
					announceData.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				SaveLocalAnnouncements();
			}
		}
	}

	private void SendChatMessage()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		if (string.IsNullOrWhiteSpace(chatInput))
		{
			return;
		}
		string text = chatInput.Trim();
		chatInput = "";
		string text2 = (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? "Unknown" : PhotonNetwork.LocalPlayer.NickName);
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.RaiseEvent((byte)80, (object)new object[3] { text2, text, actorNumber }, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
			return;
		}
		chatMessages.Add("<color=#" + ColorUtility.ToHtmlStringRGB(guiColorA) + ">" + text2 + "</color> (local): " + text);
		if (chatMessages.Count > 100)
		{
			chatMessages.RemoveAt(0);
		}
	}

	private void SendAnnouncement()
	{
		if (!string.IsNullOrWhiteSpace(announceInput))
		{
			string m = announceInput.Trim();
			announceInput = "";
			string s = (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? "Unknown" : PhotonNetwork.LocalPlayer.NickName);
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			long id = ++announceIdCounter;
			announceData.Add(new AnnounceEntry
			{
				s = s,
				m = m,
				a = actorNumber,
				id = id
			});
		}
	}

	private void AddAnnounceToRoom(string sender, string message, int actor, long id)
	{
		List<AnnounceEntry> list = LoadAnnounceEntries();
		list.Add(new AnnounceEntry
		{
			s = sender,
			m = message,
			a = actor,
			id = id
		});
		if (list.Count > 100)
		{
			list.RemoveAt(0);
		}
		SaveAnnounceEntries(list);
	}

	private void SendDeleteAnnouncement(int index)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		if (index < 0 || index >= announceData.Count)
		{
			return;
		}
		long id = announceData[index].id;
		announceData.RemoveAt(index);
		SaveLocalAnnouncements();
		if (PhotonNetwork.InRoom)
		{
			List<AnnounceEntry> list = LoadAnnounceEntries();
			list.RemoveAll((AnnounceEntry e) => e.id == id);
			SaveAnnounceEntries(list);
			PhotonNetwork.RaiseEvent((byte)82, (object)new object[1] { id }, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
		}
	}

	private List<AnnounceEntry> LoadAnnounceEntries()
	{
		if (!PhotonNetwork.InRoom)
		{
			return new List<AnnounceEntry>();
		}
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null && ((Dictionary<object, object>)(object)((RoomInfo)currentRoom).CustomProperties).TryGetValue((object)"SeralythAnnc", out object value) && value is string text && !string.IsNullOrEmpty(text))
		{
			AnnounceStorage announceStorage = JsonUtility.FromJson<AnnounceStorage>(text);
			if (announceStorage?.items != null)
			{
				return announceStorage.items;
			}
		}
		return new List<AnnounceEntry>();
	}

	private void SaveAnnounceEntries(List<AnnounceEntry> entries)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0026: Expected O, but got Unknown
		string value = JsonUtility.ToJson((object)new AnnounceStorage
		{
			items = entries
		});
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"SeralythAnnc", (object)value);
		Hashtable val2 = val;
		PhotonNetwork.CurrentRoom.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
	}

	private void SaveLocalAnnouncements()
	{
		try
		{
			string contents = JsonUtility.ToJson((object)new AnnounceStorage
			{
				items = announceData
			});
			File.WriteAllText(LocalAnnouncePath, contents);
		}
		catch
		{
		}
	}

	private void LoadLocalAnnouncements()
	{
		try
		{
			if (!File.Exists(LocalAnnouncePath))
			{
				return;
			}
			string text = File.ReadAllText(LocalAnnouncePath);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			AnnounceStorage announceStorage = JsonUtility.FromJson<AnnounceStorage>(text);
			if (announceStorage?.items == null || announceStorage.items.Count == 0)
			{
				return;
			}
			announceData = announceStorage.items;
			announceIdCounter = 0L;
			foreach (AnnounceEntry announceDatum in announceData)
			{
				if (announceDatum.id > announceIdCounter)
				{
					announceIdCounter = announceDatum.id;
				}
			}
		}
		catch
		{
		}
	}

	private void RefreshAnnouncementsFromRoom()
	{
		announceData.Clear();
		announceIdCounter = 0L;
		foreach (AnnounceEntry item in LoadAnnounceEntries())
		{
			announceData.Add(item);
			if (item.id > announceIdCounter)
			{
				announceIdCounter = item.id;
			}
		}
		SaveLocalAnnouncements();
	}

	private string FormatAnnounce(AnnounceEntry e)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		string text = ((guiColorA.r + guiColorA.g + guiColorA.b < 1.5f) ? ColorUtility.ToHtmlStringRGB(Color.cyan) : ColorUtility.ToHtmlStringRGB(guiColorA));
		string text2 = ((e.a == PhotonNetwork.LocalPlayer.ActorNumber) ? " (you)" : "");
		return "<color=#FFD700>[ANNOUNCEMENT]</color> <color=#" + text + ">" + e.s + "</color>" + text2 + ": " + e.m;
	}

	private void AddAnnounceLocal(string sender, string message, int actor, long id)
	{
		AnnounceEntry item = new AnnounceEntry
		{
			s = sender,
			m = message,
			a = actor,
			id = id
		};
		announceData.Add(item);
		if (announceData.Count > 100)
		{
			announceData.RemoveAt(0);
		}
	}

	private void DrawChatTab()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		float num = 170f;
		float num2 = 21f;
		float num3 = ((Rect)(ref guiRect)).width - 170f;
		float num4 = ((Rect)(ref guiRect)).height - 56f;
		bool inRoom = PhotonNetwork.InRoom;
		float num5 = 80f;
		GUI.backgroundColor = ((!showAnnouncements) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num5, 22f), "Chat"))
		{
			showAnnouncements = false;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = (showAnnouncements ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num + num5 + 5f, num2, num5, 22f), "Announce"))
		{
			showAnnouncements = true;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num6 = num2 + 27f;
		float num7 = 30f;
		float num8 = num4 - num6 + num2 - num7 - 10f;
		if (showAnnouncements)
		{
			if (announceData.Count == 0)
			{
				LoadLocalAnnouncements();
				if (inRoom)
				{
					RefreshAnnouncementsFromRoom();
				}
			}
			bool flag = PhotonNetwork.LocalPlayer.UserId != null && (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || ServerData.SuperAdministrators.Contains(PhotonNetwork.LocalPlayer.UserId) || ServerData.OwnerUserIds.Contains(PhotonNetwork.LocalPlayer.UserId));
			float num9 = 20f;
			float num10 = num3 - 20f;
			float num11 = Mathf.Max(num8, (float)announceData.Count * num9);
			float num12 = 25f;
			chatScrollPosition = GUI.BeginScrollView(new Rect(num, num6, num3, num8), chatScrollPosition, new Rect(0f, 0f, num10, num11), false, true);
			float num13 = 0f;
			for (int i = 0; i < announceData.Count; i++)
			{
				string text = FormatAnnounce(announceData[i]);
				float num14 = (flag ? (num10 - 35f) : (num10 - 10f));
				GUI.Label(new Rect(5f, num13, num14, num9), text);
				if (flag)
				{
					Color backgroundColor = GUI.backgroundColor;
					GUI.backgroundColor = Color.red;
					if (GUI.Button(new Rect(num10 - 120f, num13, num12, num9), "X"))
					{
						SendDeleteAnnouncement(i);
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					GUI.backgroundColor = backgroundColor;
				}
				num13 += num9;
			}
			GUI.EndScrollView();
			if (announceData.Count != prevAnnounceCount)
			{
				prevAnnounceCount = announceData.Count;
				float num15 = num11 - num8;
				if (num15 > 0f)
				{
					chatScrollPosition.y = num15;
				}
			}
			if (flag)
			{
				float num16 = num6 + num8 + 5f;
				announceInput = GUI.TextField(new Rect(num, num16, num3 - 85f, num7), announceInput);
				GUI.backgroundColor = guiColorA;
				if (GUI.Button(new Rect(num + num3 - 80f, num16, 75f, num7), "Send Annc"))
				{
					SendAnnouncement();
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = guiColorB;
			}
			return;
		}
		float num17 = 20f;
		float num18 = Mathf.Max(num8, (float)chatMessages.Count * num17);
		chatScrollPosition = GUI.BeginScrollView(new Rect(num, num6, num3, num8), chatScrollPosition, new Rect(0f, 0f, num3 - 20f, num18), false, true);
		float num19 = 0f;
		for (int j = 0; j < chatMessages.Count; j++)
		{
			GUI.Label(new Rect(5f, num19, num3 - 30f, num17), chatMessages[j]);
			num19 += num17;
		}
		GUI.EndScrollView();
		if (chatMessages.Count != prevChatCount)
		{
			prevChatCount = chatMessages.Count;
			float num20 = num18 - num8;
			if (num20 > 0f)
			{
				chatScrollPosition.y = num20;
			}
		}
		float num21 = num6 + num8 + 5f;
		chatInput = GUI.TextField(new Rect(num, num21, num3 - 85f, num7), chatInput);
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(num + num3 - 80f, num21, 75f, num7), "Send"))
		{
			SendChatMessage();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
	}

	private void DrawFriendsTab()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ede: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_105f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0995: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		float num = 170f;
		float num2 = 21f;
		float num3 = ((Rect)(ref guiRect)).width - 170f;
		float num4 = ((Rect)(ref guiRect)).height - 56f;
		float num5 = 25f;
		FriendManager.FriendsListUpdated();
		FriendManager.FriendData friendData = FriendManager.instance?.Friends;
		if (friendData == null)
		{
			GUI.Label(new Rect(num, num2 + 10f, num3, 25f), "Loading friends...");
			return;
		}
		FriendManager.FriendData.Friend[] first = (from friend2 in friendData.friends.Values
			where friend2.online
			orderby friend2.currentName
			select friend2).ToArray();
		FriendManager.FriendData.Friend[] second = (from friend2 in friendData.friends.Values
			where !friend2.online
			orderby friend2.currentName
			select friend2).ToArray();
		FriendManager.FriendData.Friend[] array = first.Concat(second).ToArray();
		int count = friendData.incoming.Count;
		int count2 = friendData.outgoing.Count;
		float num6 = (num3 - 10f) / 4f;
		GUI.backgroundColor = (string.IsNullOrEmpty(selectedFriendKey) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num6, 22f), $"Friends [{array.Length}]"))
		{
			selectedFriendKey = "";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.enabled = count > 0;
		if (GUI.Button(new Rect(num + num6 + 3f, num2, num6, 22f), $"Incoming [{count}]"))
		{
			selectedFriendKey = "__incoming__";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.enabled = true;
		GUI.enabled = count2 > 0;
		if (GUI.Button(new Rect(num + (num6 + 3f) * 2f, num2, num6, 22f), $"Outgoing [{count2}]"))
		{
			selectedFriendKey = "__outgoing__";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.enabled = true;
		if (GUI.Button(new Rect(num + (num6 + 3f) * 3f, num2, num6, 22f), "Add Friend"))
		{
			selectedFriendKey = "__add__";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		float num7 = num2 + 27f;
		float num8 = num4 - num7 + num2 - num5 - 10f;
		float num9 = 20f;
		if (selectedFriendKey == "__incoming__")
		{
			FriendManager.FriendData.PendingFriend[] array2 = friendData.incoming.Values.OrderBy((FriendManager.FriendData.PendingFriend pendingFriend3) => pendingFriend3.currentName).ToArray();
			float num10 = Mathf.Max(num8, (float)array2.Length * num9);
			friendsScrollPosition = GUI.BeginScrollView(new Rect(num, num7, num3, num8), friendsScrollPosition, new Rect(0f, 0f, num3 - 20f, num10), false, true);
			float num11 = 0f;
			for (int num12 = 0; num12 < array2.Length; num12++)
			{
				string uid = friendData.incoming.Keys.ElementAt(num12);
				FriendManager.FriendData.PendingFriend pendingFriend = array2[num12];
				GUI.Label(new Rect(5f, num11, num3 - 190f, num9), pendingFriend.currentName ?? "");
				GUI.backgroundColor = new Color(0.2f, 0.7f, 0.3f);
				if (GUI.Button(new Rect(num3 - 185f, num11, 85f, num9), "Accept"))
				{
					FriendManager.AcceptFriendRequest(uid);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = new Color(0.7f, 0.2f, 0.2f);
				if (GUI.Button(new Rect(num3 - 95f, num11, 90f, num9), "Deny"))
				{
					FriendManager.DenyFriendRequest(uid);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = guiColorB;
				num11 += num9;
			}
			GUI.EndScrollView();
			return;
		}
		if (selectedFriendKey == "__outgoing__")
		{
			FriendManager.FriendData.PendingFriend[] array3 = friendData.outgoing.Values.OrderBy((FriendManager.FriendData.PendingFriend pendingFriend3) => pendingFriend3.currentName).ToArray();
			float num13 = Mathf.Max(num8, (float)array3.Length * num9);
			friendsScrollPosition = GUI.BeginScrollView(new Rect(num, num7, num3, num8), friendsScrollPosition, new Rect(0f, 0f, num3 - 20f, num13), false, true);
			float num14 = 0f;
			for (int num15 = 0; num15 < array3.Length; num15++)
			{
				string uid2 = friendData.outgoing.Keys.ElementAt(num15);
				FriendManager.FriendData.PendingFriend pendingFriend2 = array3[num15];
				GUI.Label(new Rect(5f, num14, num3 - 100f, num9), pendingFriend2.currentName ?? "");
				GUI.backgroundColor = new Color(0.7f, 0.5f, 0.2f);
				if (GUI.Button(new Rect(num3 - 95f, num14, 90f, num9), "Cancel"))
				{
					FriendManager.CancelFriendRequest(uid2);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = guiColorB;
				num14 += num9;
			}
			GUI.EndScrollView();
			return;
		}
		if (selectedFriendKey == "__add__")
		{
			if (!PhotonNetwork.InRoom)
			{
				GUI.Label(new Rect(num, num7 + 10f, num3, 25f), "Join a room to send friend requests.");
				return;
			}
			Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
			float num16 = Mathf.Max(num8, (float)playerListOthers.Length * num9);
			friendsScrollPosition = GUI.BeginScrollView(new Rect(num, num7, num3, num8), friendsScrollPosition, new Rect(0f, 0f, num3 - 20f, num16), false, true);
			float num17 = 0f;
			Player[] array4 = playerListOthers;
			foreach (Player player in array4)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(player));
				Color color = (((Object)(object)vRRigFromPlayer != (Object)null) ? vRRigFromPlayer.playerColor : Color.white);
				bool flag = friendData.friends.Values.Any((FriendManager.FriendData.Friend friend2) => friend2.currentUserID == player.UserId);
				bool flag2 = FriendManager.instance.Friends.outgoing.ContainsKey(player.UserId) || FriendManager.instance.Friends.incoming.ContainsKey(player.UserId);
				if (!flag && !flag2)
				{
					string text = ((object)GUI.color/*cast due to .constrained prefix*/).ToString();
					GUI.color = color;
					GUI.Label(new Rect(5f, num17, num3 - 100f, num9), player.NickName);
					GUI.color = Color.white;
					GUI.backgroundColor = guiColorA;
					if (GUI.Button(new Rect(num3 - 95f, num17, 90f, num9), "Add"))
					{
						FriendManager.SendFriendRequest(player.UserId);
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					GUI.backgroundColor = guiColorB;
				}
				else
				{
					string text2 = (flag ? "(Friend)" : "(Pending)");
					GUI.Label(new Rect(5f, num17, num3 - 20f, num9), player.NickName + " " + text2);
				}
				num17 += num9;
			}
			GUI.EndScrollView();
			return;
		}
		if (string.IsNullOrEmpty(selectedFriendKey) || !friendData.friends.ContainsKey(selectedFriendKey))
		{
			float num19 = Mathf.Max(num8, (float)array.Length * num9);
			friendsScrollPosition = GUI.BeginScrollView(new Rect(num, num7, num3, num8), friendsScrollPosition, new Rect(0f, 0f, num3 - 20f, num19), false, true);
			float num20 = 0f;
			foreach (FriendManager.FriendData.Friend f in array)
			{
				string key = friendData.friends.FirstOrDefault((KeyValuePair<string, FriendManager.FriendData.Friend> kv) => kv.Value == f).Key;
				string text3 = (f.online ? "<color=green>[Online]</color>" : "<color=red>[Offline]</color>");
				string text4 = f.currentName + " " + text3;
				GUI.backgroundColor = (Color)((key == selectedFriendKey) ? guiColorA : new Color(0.15f, 0.15f, 0.2f));
				if (GUI.Button(new Rect(5f, num20, num3 - 30f, num9), text4))
				{
					selectedFriendKey = key;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = guiColorB;
				num20 += num9;
			}
			GUI.EndScrollView();
			if (array.Length == 0)
			{
				GUI.Label(new Rect(num, num7 + 10f, num3, 25f), "No friends yet. Go to Add Friend to send a request.");
			}
			return;
		}
		FriendManager.FriendData.Friend friend = friendData.friends[selectedFriendKey];
		float num22 = 22f;
		float num23 = num7;
		string text5 = (friend.online ? "<color=green>Online</color>" : "<color=red>Offline</color>");
		GUI.Label(new Rect(num, num23, num3, num22), "  " + friend.currentName + "  -  Status: " + text5);
		num23 += num22;
		if (friend.online && friend.currentRoom != "")
		{
			GUI.Label(new Rect(num, num23, num3, num22), "  Room: " + friend.currentRoom);
			num23 += num22;
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(num, num23, (num3 - 10f) / 3f, num22), "Join"))
			{
				((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(friend.currentRoom, (JoinType)0);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			if (GUI.Button(new Rect(num + (num3 - 10f) / 3f + 5f, num23, (num3 - 10f) / 3f, num22), "Invite"))
			{
				FriendManager.InviteFriend(selectedFriendKey);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			if (GUI.Button(new Rect(num + ((num3 - 10f) / 3f + 5f) * 2f, num23, (num3 - 10f) / 3f, num22), "Req Invite"))
			{
				FriendManager.RequestInviteFriend(selectedFriendKey);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			num23 += num22 + 5f;
		}
		GUI.backgroundColor = new Color(0.7f, 0.2f, 0.2f);
		if (GUI.Button(new Rect(num, num23, 100f, num22), "Remove"))
		{
			FriendManager.RemoveFriend(selectedFriendKey);
			selectedFriendKey = "";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		num23 += num22 + 5f;
		string path = "SeralythMenu/Friends/Messages/" + selectedFriendKey + ".json";
		List<string> list = new List<string>();
		if (File.Exists(path))
		{
			try
			{
				JObject val = JObject.Parse(File.ReadAllText(path));
				JToken obj = val["messages"];
				list = ((obj != null) ? obj.ToObject<List<string>>() : null) ?? new List<string>();
			}
			catch
			{
			}
		}
		float num24 = num23;
		float num25 = num7 + num8 - num24;
		if (num25 < 60f)
		{
			num25 = 60f;
		}
		GUI.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
		GUI.Box(new Rect(num - 2f, num24 - 2f, num3 + 4f, num25 + 4f), "");
		GUI.backgroundColor = guiColorB;
		float num26 = 16f;
		float num27 = Mathf.Max(num25 - num5 - 10f, (float)list.Count * num26);
		friendsScrollPosition = GUI.BeginScrollView(new Rect(num, num24, num3, num25 - num5 - 5f), friendsScrollPosition, new Rect(0f, 0f, num3 - 20f, num27), false, true);
		float num28 = 0f;
		for (int num29 = 0; num29 < list.Count; num29++)
		{
			GUI.Label(new Rect(5f, num28, num3 - 30f, num26), list[num29]);
			num28 += num26;
		}
		GUI.EndScrollView();
		float num30 = num24 + num25 - num5;
		friendsChatInput = GUI.TextField(new Rect(num, num30, num3 - 85f, num5), friendsChatInput);
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(num + num3 - 80f, num30, 75f, num5), "Send") && !string.IsNullOrEmpty(friendsChatInput.Trim()))
		{
			FriendManager.SendFriendMessage(selectedFriendKey, friendsChatInput.Trim());
			string text6 = Main.ColorToHex(VRRig.LocalRig.playerColor);
			FriendManager.UpdateFriendMessage(selectedFriendKey, "<color=grey>[</color><color=#" + text6 + ">" + PhotonNetwork.NickName.ToUpper() + "</color><color=grey>]</color> " + friendsChatInput.Trim() + "        ");
			friendsChatInput = "";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
	}

	private void DrawOnlinePlayers()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (onlinePlayers.Count != 0)
		{
			string text = "";
			for (int i = 0; i < onlinePlayers.Count; i++)
			{
				text = text + onlinePlayers[i] + "\n";
			}
			GUI.Label(new Rect(165f, 50f, 300f, 350f), text);
		}
	}

	private void DrawPlayersTab()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Expected O, but got Unknown
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected O, but got Unknown
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12df: Unknown result type (might be due to invalid IL or missing references)
		//IL_131a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1321: Unknown result type (might be due to invalid IL or missing references)
		//IL_1326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Expected O, but got Unknown
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Expected O, but got Unknown
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Expected O, but got Unknown
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1381: Unknown result type (might be due to invalid IL or missing references)
		//IL_137a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_124c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1277: Unknown result type (might be due to invalid IL or missing references)
		//IL_1287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_149d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1532: Unknown result type (might be due to invalid IL or missing references)
		//IL_1537: Unknown result type (might be due to invalid IL or missing references)
		//IL_1540: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1551: Expected O, but got Unknown
		//IL_155e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1563: Unknown result type (might be due to invalid IL or missing references)
		//IL_14eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_150c: Unknown result type (might be due to invalid IL or missing references)
		//IL_151c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1130: Unknown result type (might be due to invalid IL or missing references)
		//IL_1179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fb: Expected O, but got Unknown
		//IL_121b: Unknown result type (might be due to invalid IL or missing references)
		//IL_122d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1629: Unknown result type (might be due to invalid IL or missing references)
		//IL_162e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1637: Unknown result type (might be due to invalid IL or missing references)
		//IL_1641: Expected O, but got Unknown
		//IL_170d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf1: Expected O, but got Unknown
		//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
		if (onlinePlayers.Count == 0)
		{
			GUI.Label(new Rect(170f, 50f, 300f, 25f), "Not in a Room");
			return;
		}
		if (selectedPlayerIndex >= onlinePlayers.Count)
		{
			selectedPlayerIndex = -1;
		}
		float num = 290f;
		float num2 = 180f;
		float num3 = 170f;
		float num4 = 38f;
		float rightX = num3 + num + 10f;
		float num5 = ((Rect)(ref guiRect)).width - rightX - 10f;
		bool flag = selectedPlayerIndex >= 0;
		string[] array = new string[4] { "1st Person", "3rd Person", "In Front", "Player Video" };
		float num6 = 70f;
		float num7 = num3 + (num - num6 * 4f - 12f) / 2f;
		for (int i = 0; i < array.Length; i++)
		{
			GUI.backgroundColor = (Color)((camMode == i) ? guiColorA : new Color(0.22f, 0.22f, 0.28f));
			if (GUI.Button(new Rect(num7 + (float)i * (num6 + 4f), num4 - 16f, num6, 16f), array[i]))
			{
				camMode = i;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
		GUI.Box(new Rect(num3 - 2f, num4 - 2f, num + 4f, num2 + 4f), "");
		GUI.backgroundColor = guiColorB;
		if (flag && (Object)(object)fpRenderTexture != (Object)null && (Object)(object)fpCamera != (Object)null && ((Component)fpCamera).gameObject.activeSelf)
		{
			GUI.DrawTexture(new Rect(num3, num4, num, num2), (Texture)(object)fpRenderTexture, (ScaleMode)2);
			string text = onlinePlayers[selectedPlayerIndex];
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.7f);
			GUI.Box(new Rect(num3, num4 + num2 - 22f, num, 22f), "");
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.white;
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			val.normal.textColor = GetPlayerRoleColor(text);
			GUI.color = Color.white;
			GUI.Label(new Rect(num3, num4 + num2 - 22f, num, 22f), text + "  [" + array[camMode] + "]", val);
			GUI.color = Color.white;
		}
		else
		{
			GUI.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
			GUI.Box(new Rect(num3, num4, num, num2), "");
			GUI.backgroundColor = Color.clear;
			GUI.color = new Color(0.5f, 0.5f, 0.5f);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 11,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num3, num4 + num2 / 2f - 12f, num, 24f), "Click a player to spectate", val2);
			GUI.color = Color.white;
		}
		float panelY;
		GUIStyle infoStyle;
		GUIStyle valStyle;
		float iy;
		float labelW;
		float valX;
		float valW;
		float rowH;
		float contentH;
		if (flag)
		{
			VRRig selectedPlayerRig = GetSelectedPlayerRig();
			panelY = num4;
			GUI.backgroundColor = new Color(0.12f, 0.12f, 0.16f, 0.95f);
			GUI.Box(new Rect(rightX - 2f, panelY - 2f, num5 + 4f, num2 + 4f), "");
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				fontStyle = (FontStyle)1
			};
			infoStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10,
				richText = true
			};
			valStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10,
				richText = true,
				alignment = (TextAnchor)5
			};
			iy = panelY + 4f;
			labelW = num5 * 0.45f;
			valX = rightX + labelW;
			valW = num5 - labelW - 4f;
			rowH = 16f;
			string text2 = onlinePlayers[selectedPlayerIndex];
			bool flag2 = text2.EndsWith("(you)");
			Color backgroundColor = default(Color);
			((Color)(ref backgroundColor))._002Ector(0.3f, 0.3f, 0.35f);
			string[] array2 = new string[2] { "Player", "Room" };
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(rightX + 4f, panelY + num2 - 18f, 22f, 16f), "<"))
			{
				playerInfoPage = (playerInfoPage - 1 + array2.Length) % array2.Length;
				if (playerInfoPage == 0)
				{
					CapturePortrait(text2);
				}
			}
			GUI.backgroundColor = new Color(0.22f, 0.22f, 0.28f);
			GUI.Button(new Rect(rightX + 30f, panelY + num2 - 18f, num5 - 60f, 16f), "<size=10>" + array2[playerInfoPage] + " Info</size>");
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(rightX + num5 - 26f, panelY + num2 - 18f, 22f, 16f), ">"))
			{
				playerInfoPage = (playerInfoPage + 1) % array2.Length;
				if (playerInfoPage == 0)
				{
					CapturePortrait(text2);
				}
			}
			GUI.backgroundColor = Color.clear;
			contentH = num2 - 24f;
			if (playerInfoPage == 0)
			{
				GUI.color = guiColorA;
				GUI.Label(new Rect(rightX + 4f, iy, num5 - 8f, 18f), "<size=12><b>Player Info</b></size>", val3);
				GUI.color = Color.white;
				iy += 18f;
				GUI.backgroundColor = backgroundColor;
				GUI.Box(new Rect(rightX + 4f, iy, num5 - 8f, 1f), "");
				iy += 4f;
				if ((Object)(object)selectedPlayerRig != (Object)null)
				{
					DrawInfoRow("Name", text2);
					DrawInfoRow("Platform", selectedPlayerRig.IsSteam() ? "Steam" : "Quest");
					DrawInfoRow("Status", selectedPlayerRig.IsTagged() ? "<color=red>Tagged</color>" : "<color=green>Not Tagged</color>");
					DrawInfoRow("Ping", selectedPlayerRig.GetPing() + "ms");
					DrawInfoRow("Local", flag2 ? "Yes" : "No");
					DrawInfoRow("Muted", selectedPlayerRig.muted ? "Yes" : "No");
					Color pc = (((Object)(object)selectedPlayerRig.mainSkin != (Object)null) ? ((Renderer)selectedPlayerRig.mainSkin).material.color : selectedPlayerRig.playerColor);
					DrawColorRow("Color", pc);
					string text3 = selectedPlayerRig.Cosmetics();
					DrawInfoRow("Cosmetics", ((!string.IsNullOrEmpty(text3)) ? text3.Split(',').Length : 0) + " items");
					DrawInfoRow("Last Seen", GetLastSeen(text2));
					string playerNote = GetPlayerNote(text2);
					DrawInfoRow("Note", string.IsNullOrEmpty(playerNote) ? "(none)" : playerNote);
					if (iy <= panelY + contentH)
					{
						if (editingNoteFor == text2)
						{
							GUI.backgroundColor = new Color(0.15f, 0.15f, 0.2f);
							noteInputText = GUI.TextField(new Rect(rightX + 6f, iy, num5 - 60f, 16f), noteInputText, 64);
							GUI.backgroundColor = new Color(0.3f, 0.5f, 0.3f);
							if (GUI.Button(new Rect(rightX + num5 - 50f, iy, 22f, 16f), "OK"))
							{
								SetPlayerNote(text2, noteInputText);
								editingNoteFor = "";
								noteInputText = "";
								SoundManager.Play(SoundManager.DefaultSounds["Button"]);
							}
							GUI.backgroundColor = new Color(0.5f, 0.3f, 0.3f);
							if (GUI.Button(new Rect(rightX + num5 - 26f, iy, 22f, 16f), "X"))
							{
								SetPlayerNote(text2, "");
								editingNoteFor = "";
								noteInputText = "";
								SoundManager.Play(SoundManager.DefaultSounds["Button"]);
							}
							GUI.backgroundColor = Color.clear;
						}
						else
						{
							GUI.backgroundColor = guiColorA;
							if (GUI.Button(new Rect(rightX + 6f, iy, num5 - 12f, 16f), "<size=10>Set Note</size>"))
							{
								editingNoteFor = text2;
								noteInputText = GetPlayerNote(text2);
								SoundManager.Play(SoundManager.DefaultSounds["Button"]);
							}
							GUI.backgroundColor = Color.clear;
						}
					}
					if (iy <= panelY + contentH)
					{
						iy += 2f;
						int playerRole = GetPlayerRole(text2);
						GUIStyle val4 = new GUIStyle(GUI.skin.label)
						{
							fontSize = 10,
							richText = true
						};
						GUI.backgroundColor = Color.clear;
						GUI.color = new Color(0.7f, 0.7f, 0.75f);
						GUI.Label(new Rect(rightX + 6f, iy, labelW, rowH), "<size=10>Role</size>", val4);
						GUI.color = Color.white;
						float num8 = (num5 - 16f) / 3f;
						for (int j = 0; j < 3; j++)
						{
							GUI.backgroundColor = (Color)(j switch
							{
								0 => (playerRole == 0) ? new Color(0.35f, 0.35f, 0.4f) : new Color(0.2f, 0.2f, 0.25f), 
								1 => (playerRole == 1) ? (roleFriendColor * 0.7f) : new Color(0.15f, 0.3f, 0.15f), 
								_ => (playerRole == 2) ? (roleFoeColor * 0.7f) : new Color(0.35f, 0.15f, 0.15f), 
							});
							if (GUI.Button(new Rect(rightX + 6f + (float)j * (num8 + 2f), iy, num8, 14f), "<size=9>" + roleNames[j] + "</size>"))
							{
								SetPlayerRole(text2, (playerRole != j) ? j : 0);
								SoundManager.Play(SoundManager.DefaultSounds["Button"]);
							}
						}
						GUI.backgroundColor = Color.clear;
						iy += rowH + 1f;
					}
				}
				else if (flag2 && (Object)(object)VRRig.LocalRig != (Object)null)
				{
					VRRig localRig = VRRig.LocalRig;
					DrawInfoRow("Name", text2);
					DrawInfoRow("Platform", localRig.IsSteam() ? "Steam" : "Quest");
					DrawInfoRow("Status", localRig.IsTagged() ? "<color=red>Tagged</color>" : "<color=green>Not Tagged</color>");
					DrawInfoRow("Ping", PhotonNetwork.GetPing() + "ms");
					DrawInfoRow("Local", "Yes");
					Color pc2 = (((Object)(object)localRig.mainSkin != (Object)null) ? ((Renderer)localRig.mainSkin).material.color : localRig.playerColor);
					DrawColorRow("Color", pc2);
				}
				else
				{
					GUI.color = new Color(0.5f, 0.5f, 0.5f);
					GUI.Label(new Rect(rightX + 4f, iy, num5 - 8f, 20f), "<size=10>No player data</size>");
					GUI.color = Color.white;
				}
			}
			else
			{
				GUI.color = guiColorA;
				GUI.Label(new Rect(rightX + 4f, iy, num5 - 8f, 18f), "<size=12><b>Room Info</b></size>", val3);
				GUI.color = Color.white;
				iy += 18f;
				GUI.backgroundColor = backgroundColor;
				GUI.Box(new Rect(rightX + 4f, iy, num5 - 8f, 1f), "");
				iy += 4f;
				if (PhotonNetwork.InRoom)
				{
					DrawInfoRoom("Room", PhotonNetwork.CurrentRoom.Name);
					DrawInfoRoom("Players", PhotonNetwork.PlayerList.Length.ToString());
					DrawInfoRoom("Region", NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper());
					DrawInfoRoom("Max Players", PhotonNetwork.CurrentRoom.MaxPlayers.ToString());
					DrawInfoRoom("Visible", PhotonNetwork.CurrentRoom.IsVisible ? "Yes" : "No");
					DrawInfoRoom("Game Mode", NetworkSystem.Instance.GameModeString ?? "Unknown");
					DrawInfoRoom("Is Private", PhotonNetwork.CurrentRoom.IsVisible ? "No" : "Yes");
					object value;
					if (!((Object)(object)selectedPlayerRig != (Object)null))
					{
						value = "-";
					}
					else
					{
						Player photonPlayer = selectedPlayerRig.GetPhotonPlayer();
						value = ((photonPlayer != null) ? photonPlayer.ActorNumber.ToString() : null) ?? "?";
					}
					DrawInfoRoom("Actor Number", (string)value);
					iy += 4f;
					GUI.backgroundColor = backgroundColor;
					if (iy <= panelY + contentH)
					{
						GUI.Box(new Rect(rightX + 4f, iy, num5 - 8f, 1f), "");
					}
					iy += 4f;
					if (iy <= panelY + contentH)
					{
						GUI.color = new Color(0.5f, 0.5f, 0.5f);
						GUIStyle val5 = new GUIStyle(GUI.skin.label)
						{
							fontSize = 9,
							wordWrap = true
						};
						GUI.Label(new Rect(rightX + 6f, iy, num5 - 12f, 40f), "<size=9>Room properties update when you join or rejoin.</size>", val5);
						GUI.color = Color.white;
					}
				}
				else
				{
					GUI.color = new Color(0.5f, 0.5f, 0.5f);
					GUI.Label(new Rect(rightX + 4f, iy, num5 - 8f, 20f), "<size=10>Not in a room</size>");
					GUI.color = Color.white;
				}
			}
		}
		float num9 = num4 + num2 + 8f;
		GUI.backgroundColor = guiColorB;
		scrollPosition = GUI.BeginScrollView(new Rect(170f, num9, ((Rect)(ref guiRect)).width - 190f, ((Rect)(ref guiRect)).height - num9 - 34f), scrollPosition, new Rect(0f, 0f, ((Rect)(ref guiRect)).width - 200f, Mathf.Max(200f, (float)onlinePlayers.Count * 36f)), false, true);
		for (int k = 0; k < onlinePlayers.Count; k++)
		{
			float num10 = (float)k * 36f;
			bool flag3 = k == selectedPlayerIndex;
			string text4 = onlinePlayers[k];
			bool flag4 = text4.EndsWith("(you)");
			GUI.backgroundColor = (Color)(flag3 ? guiColorA : new Color(0.2f, 0.2f, 0.26f));
			if (GUI.Button(new Rect(0f, num10, ((Rect)(ref guiRect)).width - 200f, 32f), ""))
			{
				if (flag3)
				{
					selectedPlayerIndex = -1;
				}
				else
				{
					selectedPlayerIndex = k;
					playerInfoPage = 0;
					CapturePortrait(text4);
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			VRRig val6 = (flag4 ? VRRig.LocalRig : null);
			if (!flag4)
			{
				foreach (VRRig activeRig in VRRigCache.ActiveRigs)
				{
					if ((Object)(object)activeRig != (Object)null && !activeRig.isLocal)
					{
						NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(activeRig);
						if (playerFromVRRig != null && playerFromVRRig.NickName == text4)
						{
							val6 = activeRig;
							break;
						}
					}
				}
			}
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.white;
			if ((Object)(object)val6 != (Object)null)
			{
				Color backgroundColor2 = (((Object)(object)val6.mainSkin != (Object)null) ? ((Renderer)val6.mainSkin).material.color : val6.playerColor);
				GUI.backgroundColor = backgroundColor2;
				GUI.Box(new Rect(8f, num10 + 8f, 16f, 16f), "");
				GUI.backgroundColor = Color.clear;
			}
			GUIStyle val7 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				fontStyle = (FontStyle)(flag3 ? 1 : 0)
			};
			string playerNote2 = GetPlayerNote(text4);
			Color playerRoleColor = GetPlayerRoleColor(text4);
			string text5 = GetPlayerRole(text4) switch
			{
				2 => " <color=#F24040>♦</color>", 
				1 => " <color=#4DE666>♥</color>", 
				_ => "", 
			};
			string text6 = (string.IsNullOrEmpty(playerNote2) ? (text4 + text5) : (text4 + text5 + " <size=9><color=yellow>[" + playerNote2 + "]</color></size>"));
			val7.normal.textColor = playerRoleColor;
			GUI.Label(new Rect(30f, num10 + 4f, 300f, 24f), text6, val7);
			if ((Object)(object)val6 != (Object)null && !flag4)
			{
				GUIStyle val8 = new GUIStyle(GUI.skin.label)
				{
					fontSize = 10,
					richText = true
				};
				string text7 = (val6.IsSteam() ? "Steam" : "Quest");
				string text8 = (val6.IsTagged() ? " | <color=red>Tagged</color>" : "");
				string lastSeen = GetLastSeen(text4);
				string text9 = ((!string.IsNullOrEmpty(lastSeen)) ? (" | <color=grey>Seen " + lastSeen + "</color>") : "");
				GUI.Label(new Rect(250f, num10 + 6f, 350f, 20f), "<size=10>" + text7 + text8 + text9 + "</size>", val8);
			}
			else if (flag4)
			{
				GUI.Label(new Rect(250f, num10 + 6f, 100f, 20f), "<size=10><color=green>You</color></size>");
			}
		}
		GUI.EndScrollView();
		void DrawColorRow(string label, Color val9)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			if (!(iy > panelY + contentH))
			{
				int num11 = Mathf.RoundToInt(val9.r * 9f);
				int num12 = Mathf.RoundToInt(val9.g * 9f);
				int num13 = Mathf.RoundToInt(val9.b * 9f);
				string text10 = $"{num11},{num12},{num13}";
				GUI.backgroundColor = Color.clear;
				GUI.color = new Color(0.7f, 0.7f, 0.75f);
				GUI.Label(new Rect(rightX + 6f, iy, labelW, rowH), "<size=10>" + label + "</size>", infoStyle);
				GUI.color = Color.white;
				GUI.Label(new Rect(valX, iy, valW - 16f, rowH), "<size=10>" + text10 + "</size>", valStyle);
				GUI.backgroundColor = val9;
				GUI.Box(new Rect(valX + valW - 14f, iy + 2f, 12f, 12f), "");
				GUI.backgroundColor = Color.clear;
				iy += rowH + 1f;
			}
		}
		void DrawInfoRoom(string label, string text10)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			if (!(iy > panelY + contentH))
			{
				GUI.backgroundColor = Color.clear;
				GUI.color = new Color(0.7f, 0.7f, 0.75f);
				GUI.Label(new Rect(rightX + 6f, iy, labelW, rowH), "<size=10>" + label + "</size>", infoStyle);
				GUI.color = Color.white;
				GUI.Label(new Rect(valX, iy, valW, rowH), "<size=10>" + text10 + "</size>", valStyle);
				iy += rowH + 1f;
			}
		}
		void DrawInfoRow(string label, string text10)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			if (!(iy > panelY + contentH))
			{
				GUI.backgroundColor = Color.clear;
				GUI.color = new Color(0.7f, 0.7f, 0.75f);
				GUI.Label(new Rect(rightX + 6f, iy, labelW, rowH), "<size=10>" + label + "</size>", infoStyle);
				GUI.color = Color.white;
				GUI.Label(new Rect(valX, iy, valW, rowH), "<size=10>" + text10 + "</size>", valStyle);
				iy += rowH + 1f;
			}
		}
	}

	private void DrawPlayerColorTab()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		float num = 700f;
		playerColorScrollPosition = GUI.BeginScrollView(new Rect(170f, 21f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), playerColorScrollPosition, new Rect(0f, 0f, 480f, num), false, true);
		float num2 = 4f;
		GUI.Label(new Rect(0f, num2, 300f, 25f), "<b>Theme Selection</b>");
		num2 += 22f;
		playerColorTemplateIndex = Mathf.Clamp(playerColorTemplateIndex, 0, playerColorTemplateNames.Length - 1);
		for (int i = 0; i < playerColorTemplateNames.Length; i++)
		{
			float num3 = (float)(i % 4) * 80f;
			float num4 = num2 + (float)(i / 4) * 32f;
			GUI.backgroundColor = ((playerColorTemplateIndex == i) ? guiColorA : guiColorB);
			if (GUI.Button(new Rect(num3, num4, 75f, 28f), playerColorTemplateNames[i]))
			{
				playerColorTemplateIndex = i;
				Color val = playerColorTemplatePresets[i][0];
				Color val2 = playerColorTemplatePresets[i][1];
				colorR = val.r;
				colorG = val.g;
				colorB = val.b;
				Color.RGBToHSV(val, ref colorHue, ref colorSaturation, ref colorBrightness);
				SetPlayerColor(offlineVRRig, val);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		num2 += (float)Mathf.CeilToInt((float)playerColorTemplateNames.Length / 4f) * 32f + 8f;
		GUI.backgroundColor = guiColorB;
		Color contentColor = GUI.contentColor;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(0f, num2, 200f, 30f), "Apply Color"))
		{
			SetPlayerColor(offlineVRRig, new Color(colorR, colorG, colorB));
		}
		GUI.backgroundColor = guiColorB;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(210f, num2, 200f, 30f), "Apply & Sync HSV"))
		{
			Color val3 = default(Color);
			((Color)(ref val3))._002Ector(colorR, colorG, colorB);
			Color.RGBToHSV(val3, ref colorHue, ref colorSaturation, ref colorBrightness);
			SetPlayerColor(offlineVRRig, val3);
		}
		GUI.backgroundColor = guiColorB;
		num2 += 38f;
		GUI.Label(new Rect(0f, num2, 300f, 25f), "<b>Preset Colors</b>");
		num2 += 22f;
		Color[] array = (Color[])(object)new Color[14]
		{
			Color.red,
			Color.blue,
			Color.green,
			Color.yellow,
			new Color(0.5f, 0f, 1f),
			new Color(1f, 0.5f, 0f),
			Color.white,
			Color.black,
			Color.cyan,
			Color.magenta,
			new Color(1f, 0.8f, 0f),
			new Color(0f, 1f, 1f),
			new Color(0.8f, 0f, 0.4f),
			new Color(0.4f, 0.8f, 0f)
		};
		string[] array2 = new string[14]
		{
			"Red", "Blue", "Green", "Yellow", "Purple", "Orange", "White", "Black", "Cyan", "Pink",
			"Gold", "Aqua", "Rose", "Lime"
		};
		for (int j = 0; j < array.Length; j++)
		{
			float num5 = (float)(j % 7) * 68f;
			float num6 = num2 + (float)(j / 7) * 32f;
			GUI.backgroundColor = array[j];
			GUI.contentColor = ((array[j].r + array[j].g + array[j].b > 1.5f) ? Color.black : Color.white);
			if (GUI.Button(new Rect(num5, num6, 64f, 28f), array2[j]))
			{
				Color val4 = array[j];
				colorR = val4.r;
				colorG = val4.g;
				colorB = val4.b;
				Color.RGBToHSV(val4, ref colorHue, ref colorSaturation, ref colorBrightness);
				SetPlayerColor(offlineVRRig, val4);
			}
		}
		GUI.contentColor = contentColor;
		GUI.backgroundColor = guiColorB;
		num2 += (float)Mathf.CeilToInt((float)array.Length / 7f) * 32f + 10f;
		GUI.Label(new Rect(0f, num2, 300f, 25f), "<b>Custom Field Toggles</b>");
		num2 += 22f;
		GUI.backgroundColor = (useCustomMenuTitle ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(0f, num2, 250f, 28f), useCustomMenuTitle ? "Custom Title: ON" : "Custom Title: OFF"))
		{
			useCustomMenuTitle = !useCustomMenuTitle;
			PlayerPrefs.SetInt("PlayerColor_CustomTitle", useCustomMenuTitle ? 1 : 0);
			PlayerPrefs.SetString("PlayerColor_TitleText", customMenuTitle);
			PlayerPrefs.Save();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		num2 += 30f;
		GUI.Label(new Rect(0f, num2, 110f, 22f), "Menu Title:");
		customMenuTitle = GUI.TextField(new Rect(115f, num2, 280f, 22f), customMenuTitle);
		num2 += 28f;
		GUI.Label(new Rect(0f, num2, 300f, 25f), "Template: <color=#" + ColorUtility.ToHtmlStringRGB(guiColorA) + ">" + playerColorTemplateNames[playerColorTemplateIndex] + "</color>");
		num2 += 24f;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(0f, num2, 200f, 30f), "Save Visual Setup"))
		{
			SavePlayerColorSetup();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.backgroundColor = guiColorB;
		if (GUI.Button(new Rect(210f, num2, 200f, 30f), "Load Visual Setup"))
		{
			LoadPlayerColorSetup(offlineVRRig);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num2 += 38f;
		GUI.backgroundColor = guiColorB;
		GUI.EndScrollView();
	}

	private void DrawThemeTab()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Invalid comparison between Unknown and I4
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Invalid comparison between Unknown and I4
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Invalid comparison between Unknown and I4
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Invalid comparison between Unknown and I4
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae1: Invalid comparison between Unknown and I4
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_0990: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
		guiScrollPosition = GUI.BeginScrollView(new Rect(170f, 21f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), guiScrollPosition, new Rect(0f, 0f, 500f, 700f), false, true);
		float num = 4f;
		GUI.Label(new Rect(0f, num, 300f, 25f), "<b>Background Color</b>");
		num += 22f;
		if (GUI.Button(new Rect(0f, num, 70f, 30f), "Default"))
		{
			ApplyTheme(Color.white, Color.blue, new Color(1f, 0f, 1f), new Color(0.54f, 0.17f, 0.89f));
		}
		if (GUI.Button(new Rect(75f, num, 70f, 30f), "Red"))
		{
			ApplyTheme(Color.red, Color.white, Color.red, new Color(0.5f, 0f, 0f));
		}
		if (GUI.Button(new Rect(150f, num, 70f, 30f), "Blue"))
		{
			ApplyTheme(Color.blue, Color.white, Color.cyan, Color.blue);
		}
		if (GUI.Button(new Rect(225f, num, 70f, 30f), "Green"))
		{
			ApplyTheme(Color.green, Color.white, Color.green, new Color(0f, 0.3f, 0f));
		}
		num += 35f;
		if (GUI.Button(new Rect(0f, num, 70f, 30f), "Black"))
		{
			ApplyTheme(Color.black, Color.white, Color.grey, Color.black);
		}
		if (GUI.Button(new Rect(75f, num, 70f, 30f), "White"))
		{
			ApplyTheme(Color.white, Color.black, Color.white, Color.grey);
		}
		if (GUI.Button(new Rect(150f, num, 70f, 30f), "Purple"))
		{
			ApplyTheme(new Color(0.5f, 0f, 1f), Color.white, new Color(0.5f, 0f, 1f), new Color(0.2f, 0f, 0.5f));
		}
		if (GUI.Button(new Rect(225f, num, 70f, 30f), "Orange"))
		{
			ApplyTheme(new Color(1f, 0.5f, 0f), Color.white, new Color(1f, 0.5f, 0f), new Color(0.5f, 0.25f, 0f));
		}
		num += 35f;
		if (GUI.Button(new Rect(0f, num, 150f, 30f), "Rainbow"))
		{
			isRainbowTheme = true;
			rainbowTime = 0f;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num += 38f;
		GUI.Label(new Rect(0f, num, 300f, 25f), "<b>Button Color Style</b>");
		num += 22f;
		if (GUI.Button(new Rect(0f, num, 70f, 30f), "Black"))
		{
			Main.buttonColors[0] = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.black)
			};
			Main.buttonColors[1] = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.black)
			};
			Main.ReloadMenu();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(75f, num, 70f, 30f), "Grey"))
		{
			Main.buttonColors[0] = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.grey)
			};
			Main.buttonColors[1] = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.grey)
			};
			Main.ReloadMenu();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(150f, num, 150f, 30f), "Rainbow"))
		{
			Main.buttonColors[0] = new ExtGradient
			{
				rainbow = true
			};
			Main.buttonColors[1] = new ExtGradient
			{
				rainbow = true
			};
			Main.ReloadMenu();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num += 38f;
		GUI.Label(new Rect(0f, num, 300f, 25f), "<b>RGB Color Wheel</b>");
		num += 22f;
		if ((Object)(object)colorWheelTexture == (Object)null)
		{
			colorWheelTexture = GenerateColorWheelTexture(180);
		}
		Color val = Color.HSVToRGB(themeWheelHue, themeWheelSaturation, themeWheelBrightness);
		Color backgroundColor = GUI.backgroundColor;
		Color color = GUI.color;
		GUI.DrawTexture(new Rect(0f, num, 180f, 180f), (Texture)(object)colorWheelTexture);
		float num2 = 90f;
		float num3 = num + 90f;
		float num4 = 88f;
		float num5 = themeWheelHue * MathF.PI * 2f;
		float num6 = themeWheelSaturation * num4;
		float num7 = num2 + Mathf.Cos(num5) * num6;
		float num8 = num3 + Mathf.Sin(num5) * num6;
		GUI.color = Color.black;
		GUI.DrawTexture(new Rect(num7 - 3f, num8 - 3f, 6f, 6f), (Texture)(object)Texture2D.whiteTexture);
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(num7 - 2f, num8 - 2f, 4f, 4f), (Texture)(object)Texture2D.whiteTexture);
		GUI.color = color;
		if ((int)Event.current.type == 0 || (int)Event.current.type == 3)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			float num9 = mousePosition.x - num2;
			float num10 = mousePosition.y - num3;
			float num11 = Mathf.Sqrt(num9 * num9 + num10 * num10);
			if (num11 <= num4 + 5f)
			{
				float num12 = Mathf.Atan2(num10, num9);
				if (num12 < 0f)
				{
					num12 += MathF.PI * 2f;
				}
				themeWheelHue = num12 / (MathF.PI * 2f);
				themeWheelSaturation = Mathf.Clamp01(num11 / num4);
				val = Color.HSVToRGB(themeWheelHue, themeWheelSaturation, themeWheelBrightness);
				if ((int)Event.current.type == 0)
				{
					themeWheelDragging = true;
				}
				Event.current.Use();
			}
		}
		if ((int)Event.current.type == 1)
		{
			themeWheelDragging = false;
		}
		float num13 = 192f;
		GUI.Label(new Rect(num13, num, 130f, 20f), "H: " + Mathf.RoundToInt(themeWheelHue * 360f) + "°");
		GUI.Label(new Rect(num13, num + 18f, 130f, 20f), "S: " + Mathf.RoundToInt(themeWheelSaturation * 100f) + "%");
		GUI.Label(new Rect(num13, num + 36f, 130f, 20f), "B: " + Mathf.RoundToInt(themeWheelBrightness * 100f) + "%");
		GUI.backgroundColor = val;
		GUI.DrawTexture(new Rect(num13, num + 58f, 60f, 60f), (Texture)(object)Texture2D.whiteTexture);
		GUI.backgroundColor = backgroundColor;
		GUI.Label(new Rect(num13 + 64f, num + 72f, 100f, 20f), "#" + ColorUtility.ToHtmlStringRGB(val));
		num += 190f;
		GUI.Label(new Rect(0f, num, 300f, 25f), "<b>Color Slider Bar</b>");
		num += 22f;
		if ((Object)(object)themeBrightnessBar == (Object)null)
		{
			themeBrightnessBar = GenerateBrightnessBarTexture(400, 24, themeWheelHue, themeWheelSaturation);
		}
		if ((int)Event.current.type == 7 && (Object)(object)themeBrightnessBar != (Object)null)
		{
			Color val2 = Color.HSVToRGB(themeWheelHue, themeWheelSaturation, 0f);
			Color val3 = Color.HSVToRGB(themeWheelHue, themeWheelSaturation, 1f);
			themeBrightnessBar.SetPixel(0, 0, val2);
			themeBrightnessBar.SetPixel(((Texture)themeBrightnessBar).width - 1, 0, val3);
			for (int i = 1; i < ((Texture)themeBrightnessBar).width - 1; i++)
			{
				float num14 = (float)i / (float)(((Texture)themeBrightnessBar).width - 1);
				themeBrightnessBar.SetPixel(i, 0, Color.Lerp(val2, val3, num14));
			}
			themeBrightnessBar.Apply();
		}
		GUI.DrawTexture(new Rect(0f, num, 400f, 24f), (Texture)(object)themeBrightnessBar);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(0f, num, 400f, 24f);
		float num15 = themeWheelBrightness * 400f - 2f;
		GUI.color = Color.black;
		GUI.DrawTexture(new Rect(num15, num - 1f, 4f, 26f), (Texture)(object)Texture2D.whiteTexture);
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(num15 + 1f, num, 2f, 24f), (Texture)(object)Texture2D.whiteTexture);
		GUI.color = color;
		GUI.backgroundColor = backgroundColor;
		if (((int)Event.current.type == 0 || (int)Event.current.type == 3) && ((Rect)(ref val4)).Contains(Event.current.mousePosition))
		{
			float num16 = Mathf.Clamp01((Event.current.mousePosition.x - ((Rect)(ref val4)).x) / ((Rect)(ref val4)).width);
			if (Mathf.Abs(num16 - themeWheelBrightness) > 0.001f)
			{
				themeWheelBrightness = num16;
			}
			Event.current.Use();
		}
		num += 32f;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(0f, num, 200f, 30f), "Apply Wheel to GUI"))
		{
			guiBgColor = Color.HSVToRGB(themeWheelHue, themeWheelSaturation, themeWheelBrightness);
			Main.backgroundColor = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(guiBgColor)
			};
			Main.ReloadMenu();
			SaveThemeColor();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(210f, num, 200f, 30f), "Sync Wheel from GUI"))
		{
			Color.RGBToHSV(guiBgColor, ref themeWheelHue, ref themeWheelSaturation, ref themeWheelBrightness);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		num += 38f;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(0f, num, 300f, 30f), "Apply GUI Color"))
		{
			Main.backgroundColor = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(guiBgColor)
			};
			Main.ReloadMenu();
			SaveThemeColor();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		num += 38f;
		GUI.EndScrollView();
	}

	private void DrawCreditsTab()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		guiScrollPosition = GUI.BeginScrollView(new Rect(170f, 21f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), guiScrollPosition, new Rect(0f, 0f, 500f, 160f), false, true);
		GUI.Label(new Rect(0f, 4f, 300f, 25f), "Credits");
		GUI.Label(new Rect(0f, 34f, 300f, 25f), "Seralyth Team");
		if (GUI.Button(new Rect(0f, 54f, 100f, 25f), "GitHub"))
		{
			Application.OpenURL("https://github.com/1x1x1x1736/api");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(110f, 54f, 100f, 25f), "Discord"))
		{
			Application.OpenURL("https://discord.gg/npJTZAH3cH");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.EndScrollView();
	}

	private void DrawIconTab()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		guiScrollPosition = GUI.BeginScrollView(new Rect(170f, 21f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), guiScrollPosition, new Rect(0f, 0f, 500f, 200f), false, true);
		GUI.Label(new Rect(0f, 4f, 300f, 25f), "Icon Color");
		if (GUI.Button(new Rect(0f, 34f, 70f, 30f), "Red"))
		{
			guiIconColor = Color.red;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(75f, 34f, 70f, 30f), "Blue"))
		{
			guiIconColor = Color.blue;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(150f, 34f, 70f, 30f), "Green"))
		{
			guiIconColor = Color.green;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(225f, 34f, 70f, 30f), "Yellow"))
		{
			guiIconColor = Color.yellow;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(0f, 69f, 70f, 30f), "Purple"))
		{
			guiIconColor = new Color(0.5f, 0f, 1f);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(75f, 69f, 70f, 30f), "Orange"))
		{
			guiIconColor = new Color(1f, 0.5f, 0f);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(150f, 69f, 70f, 30f), "White"))
		{
			guiIconColor = Color.white;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(225f, 69f, 70f, 30f), "Black"))
		{
			guiIconColor = Color.black;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(0f, 104f, 70f, 30f), "Cyan"))
		{
			guiIconColor = Color.cyan;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(75f, 104f, 70f, 30f), "Pink"))
		{
			guiIconColor = Color.magenta;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.EndScrollView();
	}

	private void SaveSuggestions()
	{
		try
		{
			string contents = JsonUtility.ToJson((object)new SuggestionStorage
			{
				items = suggestionList
			});
			File.WriteAllText(SuggestionSavePath, contents);
		}
		catch
		{
		}
	}

	private void LoadSuggestions()
	{
		try
		{
			if (!File.Exists(SuggestionSavePath))
			{
				return;
			}
			string text = File.ReadAllText(SuggestionSavePath);
			if (!string.IsNullOrEmpty(text))
			{
				SuggestionStorage suggestionStorage = JsonUtility.FromJson<SuggestionStorage>(text);
				if (suggestionStorage?.items != null)
				{
					suggestionList = suggestionStorage.items;
				}
			}
		}
		catch
		{
		}
	}

	private void DrawSuggestionsTab()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		float num = 170f;
		float num2 = 21f;
		float num3 = ((Rect)(ref guiRect)).width - 170f;
		float num4 = ((Rect)(ref guiRect)).height - 56f;
		float num5 = 80f;
		GUI.backgroundColor = (showSuggestionForm ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num5, 22f), "Submit"))
		{
			showSuggestionForm = true;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = ((!showSuggestionForm) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num + num5 + 5f, num2, num5, 22f), "View All"))
		{
			showSuggestionForm = false;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num6 = num2 + 27f;
		float num7 = num4 - num6 + num2 - 10f;
		if (showSuggestionForm)
		{
			guiScrollPosition = GUI.BeginScrollView(new Rect(num, num6, num3, num7), guiScrollPosition, new Rect(0f, 0f, 480f, 360f), false, true);
			GUI.Label(new Rect(0f, 4f, 300f, 25f), "<b>Submit a Suggestion</b>");
			GUI.Label(new Rect(0f, 34f, 100f, 20f), "Title:");
			suggestionTitle = GUI.TextField(new Rect(0f, 54f, 450f, 25f), suggestionTitle);
			GUI.Label(new Rect(0f, 89f, 100f, 20f), "Message:");
			suggestionMessage = GUI.TextArea(new Rect(0f, 109f, 450f, 80f), suggestionMessage);
			GUI.Label(new Rect(0f, 199f, 100f, 20f), "Photo URL (optional):");
			suggestionPhotoUrl = GUI.TextField(new Rect(0f, 219f, 450f, 25f), suggestionPhotoUrl);
			if (!string.IsNullOrEmpty(suggestionPhotoUrl))
			{
				GUI.backgroundColor = guiColorA;
				if (GUI.Button(new Rect(0f, 254f, 100f, 25f), "Preview"))
				{
					Application.OpenURL(suggestionPhotoUrl);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = guiColorB;
			}
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(0f, 289f, 120f, 30f), "Submit Suggestion"))
			{
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				if (string.IsNullOrWhiteSpace(suggestionTitle) || string.IsNullOrWhiteSpace(suggestionMessage))
				{
					suggestionStatus = "<color=red>Please fill in both title and message.</color>";
				}
				else
				{
					SubmitSuggestion(suggestionTitle, suggestionMessage, suggestionPhotoUrl);
					suggestionTitle = "";
					suggestionMessage = "";
					suggestionPhotoUrl = "";
					suggestionStatus = "<color=green>Suggestion submitted! Thank you.</color>";
				}
			}
			GUI.backgroundColor = guiColorB;
			if (!string.IsNullOrEmpty(suggestionStatus))
			{
				GUI.Label(new Rect(0f, 329f, 450f, 25f), suggestionStatus);
			}
			GUI.EndScrollView();
			return;
		}
		float num8 = 60f;
		float num9 = Mathf.Max(num7, (float)suggestionList.Count * num8 + 20f);
		suggestionListScroll = GUI.BeginScrollView(new Rect(num, num6, num3, num7), suggestionListScroll, new Rect(0f, 0f, num3 - 20f, num9), false, true);
		if (suggestionList.Count == 0)
		{
			GUI.Label(new Rect(5f, 4f, 300f, 25f), "No suggestions yet.");
		}
		else
		{
			for (int i = 0; i < suggestionList.Count; i++)
			{
				SuggestionEntry suggestionEntry = suggestionList[i];
				float num10 = (float)i * num8;
				string text = ((suggestionEntry.actor == PhotonNetwork.LocalPlayer.ActorNumber) ? " (you)" : "");
				GUI.Label(new Rect(5f, num10, num3 - 40f, 20f), "<b>" + suggestionEntry.title + "</b>  <color=grey>by " + suggestionEntry.sender + text + "</color>");
				GUI.Label(new Rect(5f, num10 + 20f, num3 - 40f, 35f), suggestionEntry.message);
				if (!string.IsNullOrEmpty(suggestionEntry.photoUrl))
				{
					GUI.backgroundColor = guiColorA;
					if (GUI.Button(new Rect(num3 - 185f, num10 + 5f, 80f, 20f), "Copy URL"))
					{
						GUIUtility.systemCopyBuffer = suggestionEntry.photoUrl;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					if (GUI.Button(new Rect(num3 - 100f, num10 + 5f, 80f, 20f), "View Photo"))
					{
						Application.OpenURL(suggestionEntry.photoUrl);
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					GUI.backgroundColor = guiColorB;
				}
			}
		}
		GUI.EndScrollView();
	}

	private async void SubmitSuggestion(string title, string message, string photoUrl)
	{
		try
		{
			string nick = (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? "Unknown" : PhotonNetwork.LocalPlayer.NickName);
			int actor = PhotonNetwork.LocalPlayer.ActorNumber;
			SuggestionEntry entry = new SuggestionEntry
			{
				sender = nick,
				title = title,
				message = message,
				photoUrl = (photoUrl ?? ""),
				actor = actor
			};
			suggestionList.Add(entry);
			SaveSuggestions();
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.RaiseEvent((byte)82, (object)new object[5]
				{
					nick,
					title,
					message,
					photoUrl ?? "",
					actor
				}, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)1
				}, SendOptions.SendReliable);
			}
			string webhook = "https://discord.com/api/webhooks/1523079492975853679/IW5B1EshhbhK42hqkW2jOUjLQTLE96L7DI1QP5zZQPGn2m__X2DL1bb1IRkKpO1pXdMY";
			string content = "**New Suggestion**\n**Title:** " + title + "\n**Message:** " + message + "\n**From:** " + nick;
			if (!string.IsNullOrEmpty(photoUrl))
			{
				content = content + "\n**Photo:** " + photoUrl;
			}
			using HttpClient client = new HttpClient();
			StringContent payload = new StringContent("{\"content\":\"" + content.Replace("\n", "\\n").Replace("\"", "\\\"") + "\"}", Encoding.UTF8, "application/json");
			await client.PostAsync(webhook, payload);
		}
		catch
		{
		}
	}

	private void DrawShowcasesTab()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		guiScrollPosition = GUI.BeginScrollView(new Rect(170f, 21f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), guiScrollPosition, new Rect(0f, 0f, 500f, 320f), false, true);
		GUI.Label(new Rect(0f, 4f, 300f, 25f), "Showcases");
		GUI.Label(new Rect(0f, 34f, 300f, 25f), "by Lawson_VR");
		if (GUI.Button(new Rect(0f, 54f, 100f, 25f), "Watch"))
		{
			Application.OpenURL("https://youtu.be/HjEHypdM-kk?si=62ZxxAsk-HJt3qwJ");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.Label(new Rect(0f, 89f, 300f, 25f), "by DeadAndGone5451");
		if (GUI.Button(new Rect(0f, 109f, 100f, 25f), "Watch"))
		{
			Application.OpenURL("https://www.youtube.com/watch?v=pjfnZEghRtI");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.Label(new Rect(0f, 144f, 300f, 25f), "by Gorilla tag Tuts");
		if (GUI.Button(new Rect(0f, 164f, 100f, 25f), "Watch"))
		{
			Application.OpenURL("https://www.youtube.com/watch?v=tiHy4dLQYpc");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.Label(new Rect(0f, 199f, 300f, 25f), "by c00lkidd_gtag");
		if (GUI.Button(new Rect(0f, 219f, 100f, 25f), "Watch"))
		{
			Application.OpenURL("https://youtu.be/hCO8aE8O7AM?si=ct_QFSedMlWLo7Td");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.EndScrollView();
	}

	private static bool IsCosmeticWorn(string itemName)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!CosmeticsController.hasInstance)
		{
			return false;
		}
		CosmeticSet currentWornSet = ((CosmeticsController)CosmeticsController.instance).currentWornSet;
		if (currentWornSet == null || currentWornSet.items == null)
		{
			return false;
		}
		CosmeticItem val = default(CosmeticItem);
		for (int i = 0; i < currentWornSet.items.Length; i++)
		{
			ref CosmeticItem reference = ref currentWornSet.items[i];
			object obj = val;
			if (!((object)Unsafe.As<CosmeticItem, CosmeticItem>(ref reference)/*cast due to .constrained prefix*/).Equals(obj) && currentWornSet.items[i].itemName == itemName)
			{
				return true;
			}
		}
		return false;
	}

	private unsafe static void EquipCosmetic(string cosmeticName)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (CosmeticsController.hasInstance)
		{
			CosmeticItem itemFromDict = ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(cosmeticName);
			if (!((object)(*(CosmeticItem*)(&itemFromDict))/*cast due to .constrained prefix*/).Equals((object)default(CosmeticItem)))
			{
				((CosmeticsController)CosmeticsController.instance).ApplyCosmeticItemToSet(((CosmeticsController)CosmeticsController.instance).currentWornSet, itemFromDict, true, false);
				((CosmeticsController)CosmeticsController.instance).ApplyCosmeticItemToSet(VRRig.LocalRig.tryOnSet, itemFromDict, true, false);
				((CosmeticsController)CosmeticsController.instance).UpdateWornCosmetics(PhotonNetwork.InRoom);
			}
		}
	}

	private static void UnequipCosmetic(string cosmeticName)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!CosmeticsController.hasInstance)
		{
			return;
		}
		CosmeticSet currentWornSet = ((CosmeticsController)CosmeticsController.instance).currentWornSet;
		if (currentWornSet == null || currentWornSet.items == null)
		{
			return;
		}
		CosmeticItem val = default(CosmeticItem);
		for (int i = 0; i < currentWornSet.items.Length; i++)
		{
			ref CosmeticItem reference = ref currentWornSet.items[i];
			object obj = val;
			if (!((object)Unsafe.As<CosmeticItem, CosmeticItem>(ref reference)/*cast due to .constrained prefix*/).Equals(obj) && currentWornSet.items[i].itemName == cosmeticName)
			{
				currentWornSet.items[i] = val;
				break;
			}
		}
		((CosmeticsController)CosmeticsController.instance).UpdateWornCosmetics(PhotonNetwork.InRoom);
	}

	private unsafe void DrawCosmeticsTab()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		float num = 170f;
		float num2 = 21f;
		float num3 = ((Rect)(ref guiRect)).width - 170f;
		float num4 = ((Rect)(ref guiRect)).height - 56f;
		if (!CosmeticsController.hasInstance)
		{
			GUI.Label(new Rect(num, num2, num3, 25f), "Cosmetics not loaded yet.");
			return;
		}
		string[] array = new string[4] { "All", "Hat", "Face", "Badge" };
		float num5 = num3 / (float)array.Length;
		for (int i = 0; i < array.Length; i++)
		{
			GUI.backgroundColor = ((selectedCosmeticCategory == i) ? guiColorA : guiColorB);
			if (GUI.Button(new Rect(num + (float)i * num5, num2, num5 - 2f, 22f), array[i]))
			{
				selectedCosmeticCategory = i;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		num2 += 24f;
		GUI.backgroundColor = ((selectedCosmeticCategory == 4) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num3 - 2f, 22f), "Holdable"))
		{
			selectedCosmeticCategory = 4;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		num2 += 27f;
		List<CosmeticItem> allCosmetics = ((CosmeticsController)CosmeticsController.instance).allCosmetics;
		HashSet<string> playerOwnedCosmetics = VRRig.LocalRig._playerOwnedCosmetics;
		if (playerOwnedCosmetics == null || allCosmetics == null)
		{
			return;
		}
		List<CosmeticItem> list = new List<CosmeticItem>();
		foreach (CosmeticItem item in allCosmetics)
		{
			if (((object)(*(CosmeticItem*)(&item))/*cast due to .constrained prefix*/).Equals((object)default(CosmeticItem)) || item.isNullItem || !playerOwnedCosmetics.Contains(item.itemName))
			{
				continue;
			}
			if (selectedCosmeticCategory > 0)
			{
				string text = ((selectedCosmeticCategory == 4) ? "Holdable" : array[selectedCosmeticCategory]);
				CosmeticCategory itemCategory = item.itemCategory;
				if (((object)(*(CosmeticCategory*)(&itemCategory))/*cast due to .constrained prefix*/).ToString() != text)
				{
					continue;
				}
			}
			list.Add(item);
		}
		float num6 = num4 - (num2 - 21f) - 5f;
		float num7 = 25f;
		float num8 = Mathf.Max(num6, (float)list.Count * (num7 + 3f));
		cosmeticScrollPosition = GUI.BeginScrollView(new Rect(num, num2, num3, num6), cosmeticScrollPosition, new Rect(0f, 0f, num3 - 20f, num8), false, true);
		for (int j = 0; j < list.Count; j++)
		{
			CosmeticItem val = list[j];
			float num9 = (float)j * (num7 + 3f);
			bool flag = IsCosmeticWorn(val.itemName);
			string text2 = (string.IsNullOrEmpty(val.overrideDisplayName) ? val.itemName : val.overrideDisplayName);
			string text3 = ((object)(*(CosmeticCategory*)(&val.itemCategory))/*cast due to .constrained prefix*/).ToString();
			GUI.backgroundColor = (flag ? guiColorA : guiColorB);
			if (GUI.Button(new Rect(0f, num9, num3 - 115f, num7), text2 + " <color=#888888>[" + text3 + "]</color>"))
			{
				if (flag)
				{
					UnequipCosmetic(val.itemName);
				}
				else
				{
					EquipCosmetic(val.itemName);
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = (flag ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.6f, 0.2f, 0.2f));
			GUI.Label(new Rect(num3 - 110f, num9, 70f, num7), flag ? "Worn" : "");
			GUI.backgroundColor = guiColorB;
		}
		GUI.EndScrollView();
		int num10 = 0;
		CosmeticSet currentWornSet = ((CosmeticsController)CosmeticsController.instance).currentWornSet;
		if (currentWornSet != null && currentWornSet.items != null)
		{
			for (int k = 0; k < currentWornSet.items.Length; k++)
			{
				ref CosmeticItem reference = ref currentWornSet.items[k];
				object obj = (object)default(CosmeticItem);
				if (!((object)Unsafe.As<CosmeticItem, CosmeticItem>(ref reference)/*cast due to .constrained prefix*/).Equals(obj))
				{
					num10++;
				}
			}
		}
		GUI.Label(new Rect(num, num2 + num6 + 2f, num3, 18f), "Owned: " + list.Count + " | Worn: " + num10);
	}

	private void DrawAITab()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Expected O, but got Unknown
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Expected O, but got Unknown
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Invalid comparison between Unknown and I4
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Invalid comparison between Unknown and I4
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e9: Expected O, but got Unknown
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Invalid comparison between Unknown and I4
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Invalid comparison between Unknown and I4
		float num = 170f;
		float num2 = 21f;
		float num3 = ((Rect)(ref guiRect)).width - 170f;
		float num4 = ((Rect)(ref guiRect)).height - 56f;
		float num5 = 80f;
		GUI.backgroundColor = ((!showAICmds) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num5, 22f), "Chat"))
		{
			showAICmds = false;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = (showAICmds ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num + num5 + 5f, num2, num5, 22f), "Commands"))
		{
			showAICmds = true;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num6 = num2 + 27f;
		float num7 = 30f;
		float num8 = num4 - num6 + num2 - num7 - 10f;
		float num9 = num6 + num8 + 5f;
		if (showAICmds)
		{
			if (aiChatVersion != aiChatMessages.Count)
			{
				aiChatVersion = aiChatMessages.Count;
				aiChatHeights.Clear();
				aiChatContentH = num8;
				for (int i = 0; i < aiChatMessages.Count; i++)
				{
					float num10 = GUI.skin.label.CalcHeight(new GUIContent(aiChatMessages[i]), num3 - 30f);
					aiChatHeights.Add(num10);
					aiChatContentH += num10 + 5f;
				}
			}
			if (aiScrollToBottom)
			{
				aiChatScrollPosition.y = Mathf.Max(0f, aiChatContentH - num8);
			}
			aiScrollToBottom = false;
			aiChatScrollPosition = GUI.BeginScrollView(new Rect(num, num6, num3, num8), aiChatScrollPosition, new Rect(0f, 0f, num3 - 20f, aiChatContentH));
			float num11 = 5f;
			for (int j = 0; j < aiChatMessages.Count; j++)
			{
				string text = (aiChatMessages[j].StartsWith(">") ? "lime" : "#ffb6c1");
				string text2 = "<color=" + text + ">" + aiChatMessages[j] + "</color>";
				float num12 = GUI.skin.label.CalcHeight(new GUIContent(text2), num3 - 30f);
				GUI.Label(new Rect(5f, num11, num3 - 30f, num12), text2);
				num11 += aiChatHeights[j] + 5f;
			}
			GUI.EndScrollView();
			GUI.SetNextControlName("AIChatField");
			aiChatInput = GUI.TextField(new Rect(num, num9, num3 - 70f, num7), aiChatInput, 500);
			if ((GUI.Button(new Rect(num + num3 - 65f, num9, 65f, num7), "Send") || ((int)Event.current.type == 4 && (int)Event.current.keyCode == 13 && GUI.GetNameOfFocusedControl() == "AIChatField")) && !string.IsNullOrWhiteSpace(aiChatInput))
			{
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				string text3 = aiChatInput.Trim();
				aiChatInput = "";
				aiChatMessages.Add("> " + text3);
				aiScrollToBottom = true;
				GUI.FocusControl((string)null);
				if (text3 == "/help" || text3 == "/cmds" || text3 == "/commands")
				{
					aiChatMessages.Add("Available commands: type a mod name to toggle it, or use /list to see all mods");
				}
				else if (text3 == "/list")
				{
					string text4 = "";
					ButtonInfo[][] buttons = Buttons.buttons;
					foreach (ButtonInfo[] array in buttons)
					{
						ButtonInfo[] array2 = array;
						foreach (ButtonInfo buttonInfo in array2)
						{
							if (!buttonInfo.label && buttonInfo.isTogglable)
							{
								string text5 = buttonInfo.overlapText ?? buttonInfo.buttonText;
								if (text5.Contains(" <color"))
								{
									text5 = text5.Split(" <color")[0];
								}
								text4 = text4 + text5 + ", ";
							}
						}
					}
					aiChatMessages.Add(text4.TrimEnd(',', ' '));
				}
				else
				{
					string text6 = null;
					bool flag = false;
					ButtonInfo[][] buttons2 = Buttons.buttons;
					foreach (ButtonInfo[] array3 in buttons2)
					{
						if (flag)
						{
							break;
						}
						ButtonInfo[] array4 = array3;
						foreach (ButtonInfo buttonInfo2 in array4)
						{
							if (flag)
							{
								break;
							}
							string text7 = buttonInfo2.overlapText ?? buttonInfo2.buttonText;
							if (text7.Contains(" <color"))
							{
								text7 = text7.Split(" <color")[0];
							}
							if (text3.ToLower() == text7.ToLower())
							{
								text6 = buttonInfo2.buttonText;
								flag = true;
							}
							else if (text3.ToLower().Contains(text7.ToLower()))
							{
								text6 = buttonInfo2.buttonText;
							}
						}
					}
					if (text6 != null)
					{
						ButtonInfo index = Buttons.GetIndex(text6);
						if (index != null)
						{
							bool enabled = index.enabled;
							Main.Toggle(text6, fromMenu: true);
							aiChatMessages.Add((enabled ? "Disabled " : "Enabled ") + (index.overlapText ?? index.buttonText));
						}
					}
					else
					{
						aiChatMessages.Add("No mod found matching \"" + text3 + "\". Type /help for commands.");
					}
				}
			}
			if (aiChatMessages.Count == 0)
			{
				aiChatMessages.Add("Type a mod name to toggle it, or /help for commands");
			}
			return;
		}
		if (aiMessages.Count == 0)
		{
			aiMessages.Add("<color=#00FF88>Seralyth AI:</color> Hello! I'm your Seralyth Remake AI assistant. How can I help?");
			aiScrollToBottom = true;
		}
		if (aiVersion != aiMessages.Count)
		{
			aiVersion = aiMessages.Count;
			aiHeights.Clear();
			aiContentH = num8;
			for (int num13 = 0; num13 < aiMessages.Count; num13++)
			{
				float num14 = GUI.skin.label.CalcHeight(new GUIContent(aiMessages[num13]), num3 - 30f);
				aiHeights.Add(num14);
				aiContentH += num14 + 5f;
			}
		}
		else if (aiThinking && aiMessages.Count > 0)
		{
			int index2 = aiMessages.Count - 1;
			float num15 = aiHeights[index2];
			float num16 = GUI.skin.label.CalcHeight(new GUIContent(aiMessages[index2]), num3 - 30f);
			aiContentH += num16 - num15;
			aiHeights[index2] = num16;
		}
		if (aiScrollToBottom)
		{
			aiScrollToBottom = false;
		}
		aiChatScrollPosition = GUI.BeginScrollView(new Rect(num, num6, num3, num8), aiChatScrollPosition, new Rect(0f, 0f, num3 - 20f, aiContentH));
		float num17 = 5f;
		for (int num18 = 0; num18 < aiMessages.Count; num18++)
		{
			float num19 = GUI.skin.label.CalcHeight(new GUIContent(aiMessages[num18]), num3 - 30f);
			GUI.Label(new Rect(5f, num17, num3 - 30f, num19), aiMessages[num18]);
			num17 += aiHeights[num18] + 5f;
		}
		GUI.EndScrollView();
		if (aiThinking)
		{
			aiThinkingTimer += Time.deltaTime;
			string text8 = new string('.', Mathf.FloorToInt(aiThinkingTimer * 2f) % 4);
			GUI.Label(new Rect(num, num9 - 20f, num3, 20f), "<color=grey>Seralyth AI is thinking" + text8 + "</color>");
		}
		GUI.SetNextControlName("AIChatField");
		aiInput = GUI.TextField(new Rect(num, num9, num3 - 70f, num7), aiInput, 500);
		if (GUI.Button(new Rect(num + num3 - 65f, num9, 65f, num7), "Send") || ((int)Event.current.type == 4 && (int)Event.current.keyCode == 13 && GUI.GetNameOfFocusedControl() == "AIChatField"))
		{
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			SendAIMessage();
		}
	}

	private void SendAIMessage()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrWhiteSpace(aiInput) && !aiThinking)
		{
			string text = aiInput.Trim();
			aiInput = "";
			string text2 = (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? "You" : PhotonNetwork.LocalPlayer.NickName);
			aiMessages.Add("<color=#" + ColorUtility.ToHtmlStringRGB(guiColorA) + ">" + text2 + ":</color> " + text);
			aiScrollToBottom = true;
			aiThinking = true;
			aiThinkingTimer = 0f;
			((MonoBehaviour)Instance).StartCoroutine(AskAICoroutine(text));
		}
	}

	private IEnumerator AskAICoroutine(string text)
	{
		string encoded = Uri.EscapeDataString(text);
		string prompt = Uri.EscapeDataString(AIManager.SystemPrompt);
		string api = "https://text.pollinations.ai/" + encoded + "?system=" + prompt + "&private=true&model=openai";
		UnityWebRequest request = UnityWebRequest.Get(api);
		try
		{
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			yield return request.SendWebRequest();
			if ((int)request.result != 1)
			{
				string responseText = "<color=#FF4444>Seralyth AI:</color> Error: " + request.error;
				aiMessages.Add(responseText);
			}
			else
			{
				string response = request.downloadHandler.text;
				string clean = Regex.Replace(response, "<([A-Z]+)(?:_\"[^\"]*\")?>", "").Replace("\n", "").Replace("\r", "");
				aiMessages.Add("");
				int idx = aiMessages.Count - 1;
				string prefix = "<color=#00FF88>Seralyth AI:</color> ";
				for (int i = 0; i < clean.Length; i++)
				{
					aiMessages[idx] = prefix + clean.Substring(0, i + 1);
					yield return (object)new WaitForSeconds(0.03f);
				}
			}
			aiThinking = false;
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	private void SetPlayerColor(VRRig rig, Color color)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)rig.mainSkin).material.color = color;
		rig.playerColor = color;
		SoundManager.Play(SoundManager.DefaultSounds["Button"]);
	}

	private Texture2D GenerateColorWheelTexture(int size)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(size, size, (TextureFormat)4, false);
		((Texture)val).filterMode = (FilterMode)1;
		float num = (float)size / 2f;
		float num2 = num - 1f;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float num3 = (float)j - num;
				float num4 = (float)i - num;
				float num5 = Mathf.Sqrt(num3 * num3 + num4 * num4);
				if (num5 <= num2)
				{
					float num6 = Mathf.Atan2(num4, num3);
					if (num6 < 0f)
					{
						num6 += MathF.PI * 2f;
					}
					float num7 = num6 / (MathF.PI * 2f);
					float num8 = num5 / num2;
					Color val2 = Color.HSVToRGB(num7, num8, 1f);
					val.SetPixel(j, i, val2);
				}
				else
				{
					val.SetPixel(j, i, new Color(0f, 0f, 0f, 0f));
				}
			}
		}
		val.Apply();
		return val;
	}

	private Texture2D GenerateBrightnessBarTexture(int width, int height, float hue, float saturation)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(width, height, (TextureFormat)4, false);
		((Texture)val).filterMode = (FilterMode)1;
		for (int i = 0; i < width; i++)
		{
			float num = (float)i / (float)(width - 1);
			Color val2 = Color.HSVToRGB(hue, saturation, num);
			for (int j = 0; j < height; j++)
			{
				val.SetPixel(i, j, val2);
			}
		}
		val.Apply();
		return val;
	}

	private void SavePlayerColorSetup()
	{
		PlayerPrefs.SetFloat("PlayerColor_R", colorR);
		PlayerPrefs.SetFloat("PlayerColor_G", colorG);
		PlayerPrefs.SetFloat("PlayerColor_B", colorB);
		PlayerPrefs.SetFloat("PlayerColor_H", colorHue);
		PlayerPrefs.SetFloat("PlayerColor_S", colorSaturation);
		PlayerPrefs.SetFloat("PlayerColor_Br", colorBrightness);
		PlayerPrefs.SetInt("PlayerColor_Template", playerColorTemplateIndex);
		PlayerPrefs.SetInt("PlayerColor_CustomTitle", useCustomMenuTitle ? 1 : 0);
		PlayerPrefs.SetString("PlayerColor_TitleText", customMenuTitle);
		PlayerPrefs.Save();
	}

	private Texture2D GenerateRoundedCornerTexture(int radius)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(radius, radius, (TextureFormat)4, false);
		for (int i = 0; i < radius; i++)
		{
			for (int j = 0; j < radius; j++)
			{
				float num = radius - 1 - j;
				float num2 = radius - 1 - i;
				float num3 = Mathf.Sqrt(num * num + num2 * num2);
				if (num3 > (float)(radius - 1))
				{
					val.SetPixel(j, i, Color.white);
				}
				else
				{
					val.SetPixel(j, i, Color.clear);
				}
			}
		}
		val.Apply();
		return val;
	}

	private void DrawRoundedCorners(float w, float h)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)roundedCornerTex == (Object)null)
		{
			roundedCornerTex = GenerateRoundedCornerTexture(10);
		}
		Color color = GUI.color;
		GUI.color = guiBgColor;
		GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, 10f, 10f), (Texture)(object)roundedCornerTex, new Rect(0f, 0f, 1f, 1f));
		GUI.DrawTextureWithTexCoords(new Rect(w - 10f, 0f, 10f, 10f), (Texture)(object)roundedCornerTex, new Rect(1f, 0f, -1f, 1f));
		GUI.DrawTextureWithTexCoords(new Rect(0f, h - 10f, 10f, 10f), (Texture)(object)roundedCornerTex, new Rect(0f, 1f, 1f, -1f));
		GUI.DrawTextureWithTexCoords(new Rect(w - 10f, h - 10f, 10f, 10f), (Texture)(object)roundedCornerTex, new Rect(1f, 1f, -1f, -1f));
		GUI.color = color;
	}

	private void LoadPlayerColorSetup(VRRig rig)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		colorR = PlayerPrefs.GetFloat("PlayerColor_R", 1f);
		colorG = PlayerPrefs.GetFloat("PlayerColor_G", 1f);
		colorB = PlayerPrefs.GetFloat("PlayerColor_B", 1f);
		colorHue = PlayerPrefs.GetFloat("PlayerColor_H", 0f);
		colorSaturation = PlayerPrefs.GetFloat("PlayerColor_S", 1f);
		colorBrightness = PlayerPrefs.GetFloat("PlayerColor_Br", 1f);
		playerColorTemplateIndex = PlayerPrefs.GetInt("PlayerColor_Template", 0);
		useCustomMenuTitle = PlayerPrefs.GetInt("PlayerColor_CustomTitle", 0) == 1;
		customMenuTitle = PlayerPrefs.GetString("PlayerColor_TitleText", "MrChicken Menu");
		brightnessBarTexture = null;
		SetPlayerColor(rig, new Color(colorR, colorG, colorB));
	}

	private void StartPlayerMacroRecording(int rigIndex)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		if (activeRigs != null && rigIndex < activeRigs.Count && !((Object)(object)activeRigs[rigIndex] == (Object)null))
		{
			VRRig val = activeRigs[rigIndex];
			string name = val.GetName();
			Color val2 = (((Object)(object)val.mainSkin != (Object)null) ? ((Renderer)val.mainSkin).material.color : val.playerColor);
			isRecordingPlayerMacro = true;
			recordingPlayerName = name;
			recordingPlayerColor = val2;
			currentRecordingSteps.Clear();
			macroRecordStartTime = Time.time;
			macroLastRecordTime = 0f;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
	}

	private void StopPlayerMacroRecording()
	{
		if (isRecordingPlayerMacro)
		{
			isRecordingPlayerMacro = false;
			playerMacroStore[recordingPlayerName] = new List<PlayerMacroStep>(currentRecordingSteps);
			currentRecordingSteps.Clear();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			NotificationManager.SendNotification($"<color=grey>[</color><color=yellow>MACRO</color><color=grey>]</color> Saved {playerMacroStore[recordingPlayerName].Count} steps for {recordingPlayerName}.");
		}
	}

	private void StartPlayerMacroPlayback(int rigIndex)
	{
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		if (activeRigs != null && rigIndex < activeRigs.Count && !((Object)(object)activeRigs[rigIndex] == (Object)null))
		{
			VRRig rig = activeRigs[rigIndex];
			string name = rig.GetName();
			if (!playerMacroStore.ContainsKey(name) || playerMacroStore[name].Count == 0)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>MACRO</color><color=grey>]</color> No macro recorded for " + name + ".");
				return;
			}
			isPlayingPlayerMacro = true;
			playingMacroPlayerName = name;
			macroPlayIndex = 0;
			macroPlayNextTime = Time.time;
			macroPlaybackTarget = name;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
	}

	private void StopPlayerMacroPlayback()
	{
		isPlayingPlayerMacro = false;
		playingMacroPlayerName = "";
		macroPlayIndex = 0;
		SoundManager.Play(SoundManager.DefaultSounds["Button"]);
	}

	private int GetStoredMacroCount(string playerName)
	{
		if (playerMacroStore.ContainsKey(playerName))
		{
			return playerMacroStore[playerName].Count;
		}
		return 0;
	}

	private void ClearAllPlayerMacros()
	{
		playerMacroStore.Clear();
		SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>MACRO</color><color=grey>]</color> Cleared all player macros.");
	}

	private void SavePlayerData()
	{
		try
		{
			PlayerDataFile playerDataFile = new PlayerDataFile();
			foreach (KeyValuePair<string, string> playerNote in playerNotes)
			{
				playerDataFile.notes.Add(new PlayerDataEntry
				{
					key = playerNote.Key,
					value = playerNote.Value
				});
			}
			foreach (KeyValuePair<string, string> item in playerLastSeen)
			{
				playerDataFile.lastSeen.Add(new PlayerDataEntry
				{
					key = item.Key,
					value = item.Value
				});
			}
			foreach (KeyValuePair<string, int> playerRole in playerRoles)
			{
				playerDataFile.roles.Add(new PlayerDataEntry
				{
					key = playerRole.Key,
					value = playerRole.Value.ToString()
				});
			}
			string contents = JsonUtility.ToJson((object)playerDataFile, true);
			File.WriteAllText(PlayerDataPath, contents);
		}
		catch
		{
		}
	}

	private void LoadPlayerData()
	{
		try
		{
			if (!File.Exists(PlayerDataPath))
			{
				return;
			}
			string text = File.ReadAllText(PlayerDataPath);
			PlayerDataFile playerDataFile = JsonUtility.FromJson<PlayerDataFile>(text);
			if (playerDataFile == null)
			{
				return;
			}
			playerNotes.Clear();
			playerLastSeen.Clear();
			playerRoles.Clear();
			if (playerDataFile.notes != null)
			{
				foreach (PlayerDataEntry note in playerDataFile.notes)
				{
					if (!string.IsNullOrEmpty(note.key))
					{
						playerNotes[note.key] = note.value;
					}
				}
			}
			if (playerDataFile.lastSeen != null)
			{
				foreach (PlayerDataEntry item in playerDataFile.lastSeen)
				{
					if (!string.IsNullOrEmpty(item.key))
					{
						playerLastSeen[item.key] = item.value;
					}
				}
			}
			if (playerDataFile.roles == null)
			{
				return;
			}
			foreach (PlayerDataEntry role in playerDataFile.roles)
			{
				if (!string.IsNullOrEmpty(role.key) && int.TryParse(role.value, out var result))
				{
					playerRoles[role.key] = result;
				}
			}
		}
		catch
		{
		}
	}

	private void UpdateLastSeen(string playerName)
	{
		if (!string.IsNullOrEmpty(playerName))
		{
			playerLastSeen[playerName] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
			SavePlayerData();
		}
	}

	private void SetPlayerNote(string playerName, string note)
	{
		if (!string.IsNullOrEmpty(playerName))
		{
			if (string.IsNullOrEmpty(note))
			{
				playerNotes.Remove(playerName);
			}
			else
			{
				playerNotes[playerName] = note;
			}
			SavePlayerData();
		}
	}

	private string GetPlayerNote(string playerName)
	{
		if (string.IsNullOrEmpty(playerName))
		{
			return "";
		}
		return playerNotes.ContainsKey(playerName) ? playerNotes[playerName] : "";
	}

	private string GetLastSeen(string playerName)
	{
		if (string.IsNullOrEmpty(playerName))
		{
			return "";
		}
		return playerLastSeen.ContainsKey(playerName) ? playerLastSeen[playerName] : "";
	}

	private int GetPlayerRole(string playerName)
	{
		if (string.IsNullOrEmpty(playerName))
		{
			return 0;
		}
		return playerRoles.ContainsKey(playerName) ? playerRoles[playerName] : 0;
	}

	private void SetPlayerRole(string playerName, int role)
	{
		if (!string.IsNullOrEmpty(playerName))
		{
			if (role == 0)
			{
				playerRoles.Remove(playerName);
			}
			else
			{
				playerRoles[playerName] = role;
			}
			SavePlayerData();
		}
	}

	private Color GetPlayerRoleColor(string playerName)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(GetPlayerRole(playerName) switch
		{
			1 => roleFriendColor, 
			2 => roleFoeColor, 
			_ => Color.white, 
		});
	}

	private void RecordPlayerMacroStep()
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		if (!isRecordingPlayerMacro || Time.time - macroLastRecordTime < 0.05f)
		{
			return;
		}
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		VRRig val = null;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			if ((Object)(object)activeRigs[i] != (Object)null && activeRigs[i].GetName() == recordingPlayerName)
			{
				val = activeRigs[i];
				break;
			}
		}
		if (!((Object)(object)val == (Object)null))
		{
			PlayerMacroStep item = new PlayerMacroStep
			{
				time = Time.time - macroRecordStartTime,
				headPos = (((Object)(object)val.headMesh != (Object)null) ? val.headMesh.transform.position : ((Component)val).transform.position),
				headRot = (((Object)(object)val.headMesh != (Object)null) ? val.headMesh.transform.rotation : ((Component)val).transform.rotation),
				leftHandPos = (((Object)(object)val.leftHandTransform != (Object)null) ? val.leftHandTransform.position : Vector3.zero),
				rightHandPos = (((Object)(object)val.rightHandTransform != (Object)null) ? val.rightHandTransform.position : Vector3.zero),
				leftGrab = false,
				rightGrab = false
			};
			currentRecordingSteps.Add(item);
			macroLastRecordTime = Time.time;
		}
	}

	private void PlayPlayerMacroStep()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (!isPlayingPlayerMacro)
		{
			return;
		}
		if (!playerMacroStore.ContainsKey(macroPlaybackTarget) || macroPlayIndex >= playerMacroStore[macroPlaybackTarget].Count)
		{
			StopPlayerMacroPlayback();
		}
		else if (!(Time.time < macroPlayNextTime))
		{
			PlayerMacroStep playerMacroStep = playerMacroStore[macroPlaybackTarget][macroPlayIndex];
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if ((Object)(object)offlineVRRig != (Object)null && (Object)(object)offlineVRRig.headMesh != (Object)null)
			{
				offlineVRRig.headMesh.transform.position = playerMacroStep.headPos;
				offlineVRRig.headMesh.transform.rotation = playerMacroStep.headRot;
			}
			macroPlayIndex++;
			if (macroPlayIndex < playerMacroStore[macroPlaybackTarget].Count)
			{
				float num = playerMacroStore[macroPlaybackTarget][macroPlayIndex].time - playerMacroStep.time;
				macroPlayNextTime = Time.time + Mathf.Max(num, 0.01f);
			}
			else
			{
				StopPlayerMacroPlayback();
				NotificationManager.SendNotification("<color=grey>[</color><color=yellow>MACRO</color><color=grey>]</color> Finished playing macro for " + macroPlaybackTarget + ".");
			}
		}
	}

	private void ApplyTheme(Color bg, Color content, Color a, Color b)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		isRainbowTheme = false;
		guiBgColor = bg;
		guiContentColor = content;
		guiColorA = a;
		guiColorB = b;
		SoundManager.Play(SoundManager.DefaultSounds["Button"]);
	}

	private static void SaveThemeColor()
	{
		PlayerPrefs.SetFloat("guiBgR", guiBgColor.r);
		PlayerPrefs.SetFloat("guiBgG", guiBgColor.g);
		PlayerPrefs.SetFloat("guiBgB", guiBgColor.b);
		PlayerPrefs.SetFloat("guiContentR", guiContentColor.r);
		PlayerPrefs.SetFloat("guiContentG", guiContentColor.g);
		PlayerPrefs.SetFloat("guiContentB", guiContentColor.b);
		PlayerPrefs.SetFloat("guiColorAR", guiColorA.r);
		PlayerPrefs.SetFloat("guiColorAG", guiColorA.g);
		PlayerPrefs.SetFloat("guiColorAB", guiColorA.b);
		PlayerPrefs.SetFloat("guiColorBR", guiColorB.r);
		PlayerPrefs.SetFloat("guiColorBG", guiColorB.g);
		PlayerPrefs.SetFloat("guiColorBB", guiColorB.b);
		PlayerPrefs.Save();
	}

	private void LoadReviews()
	{
		try
		{
			if (!File.Exists(LocalReviewPath))
			{
				return;
			}
			string text = File.ReadAllText(LocalReviewPath);
			if (!string.IsNullOrEmpty(text))
			{
				ReviewStorage reviewStorage = JsonUtility.FromJson<ReviewStorage>(text);
				if (reviewStorage?.items != null)
				{
					reviewEntries = reviewStorage.items;
				}
			}
		}
		catch
		{
		}
	}

	private void SaveReviews()
	{
		try
		{
			string contents = JsonUtility.ToJson((object)new ReviewStorage
			{
				items = reviewEntries
			});
			File.WriteAllText(LocalReviewPath, contents);
			string path = Path.Combine(Application.persistentDataPath, "SeralythReviews.txt");
			using StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.WriteLine("===== Seralyth Reviews =====");
			streamWriter.WriteLine();
			foreach (ReviewEntry reviewEntry in reviewEntries)
			{
				string text = "";
				for (int i = 0; i < 5; i++)
				{
					text += ((i < reviewEntry.rating) ? "★" : "☆");
				}
				streamWriter.WriteLine($"{text} ({reviewEntry.rating}/5)  -  {reviewEntry.name}  [{reviewEntry.timestamp}]");
				if (!string.IsNullOrEmpty(reviewEntry.comment))
				{
					streamWriter.WriteLine("  Comment: " + reviewEntry.comment);
				}
				streamWriter.WriteLine();
			}
		}
		catch
		{
		}
	}

	private async void SendReviewToDiscord(ReviewEntry entry)
	{
		try
		{
			string stars = "";
			for (int i = 0; i < 5; i++)
			{
				stars += ((i < entry.rating) ? "★" : "☆");
			}
			string content = string.Format("**New Review**\n**From:** {0}\n**Rating:** {1} ({2}/5)\n**Comment:** {3}\n**Time:** {4}", entry.name, stars, entry.rating, string.IsNullOrEmpty(entry.comment) ? "(none)" : entry.comment, entry.timestamp);
			string webhook = "https://discord.com/api/webhooks/1526373977008898048/laHJMbI4TY3iP7Q5dYRFjkBoKglHp6UelYS6GIl_auXGoW-GCkUPlpTyrvXVZ1aw7C_Q";
			using HttpClient client = new HttpClient();
			StringContent payload = new StringContent("{\"content\":\"" + content.Replace("\n", "\\n").Replace("\"", "\\\"") + "\"}", Encoding.UTF8, "application/json");
			await client.PostAsync(webhook, payload);
		}
		catch
		{
		}
	}

	private void DrawReviewTab()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Expected O, but got Unknown
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Expected O, but got Unknown
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Expected O, but got Unknown
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Expected O, but got Unknown
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Expected O, but got Unknown
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Expected O, but got Unknown
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		float num = 170f;
		float num2 = 21f;
		float num3 = ((Rect)(ref guiRect)).width - 180f;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 16,
			fontStyle = (FontStyle)1,
			richText = true
		};
		GUI.Label(new Rect(num, num2, num3, 30f), "Rate Seralyth Remake", val);
		num2 += 35f;
		GUIStyle val2 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 12,
			wordWrap = true,
			richText = true
		};
		GUI.Label(new Rect(num, num2, num3, 20f), "How would you rate this menu?", val2);
		num2 += 30f;
		float num4 = 50f;
		float num5 = 8f;
		float num6 = num;
		for (int i = 1; i <= 5; i++)
		{
			bool flag = reviewRating >= i;
			GUI.backgroundColor = (flag ? guiColorA : guiColorB);
			string text = (flag ? "★" : "☆");
			if (GUI.Button(new Rect(num6 + (float)(i - 1) * (num4 + num5), num2, num4, num4), text))
			{
				reviewRating = i;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		num2 += num4 + 15f;
		string[] array = new string[6] { "", "Terrible", "Bad", "Okay", "Good", "Excellent" };
		if (reviewRating > 0)
		{
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4,
				richText = true
			};
			val3.normal.textColor = guiColorA;
			GUI.Label(new Rect(num, num2, num3, 25f), array[reviewRating], val3);
			num2 += 35f;
		}
		GUI.Label(new Rect(num, num2, num3, 20f), "Your Name:", val2);
		num2 += 22f;
		reviewName = GUI.TextField(new Rect(num, num2, num3, 25f), reviewName);
		num2 += 35f;
		GUI.Label(new Rect(num, num2, num3, 20f), "Comment (optional):", val2);
		num2 += 22f;
		reviewComment = GUI.TextField(new Rect(num, num2, num3, 50f), reviewComment);
		num2 += 65f;
		GUI.backgroundColor = guiColorA;
		GUI.enabled = reviewRating > 0 && reviewName.Trim().Length > 0;
		if (GUI.Button(new Rect(num, num2, 200f, 30f), "Submit Review"))
		{
			ReviewEntry reviewEntry = new ReviewEntry
			{
				name = reviewName.Trim(),
				rating = reviewRating,
				comment = reviewComment.Trim(),
				timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
			};
			reviewEntries.Add(reviewEntry);
			SaveReviews();
			SendReviewToDiscord(reviewEntry);
			reviewSubmitResult = "Review submitted! You rated " + reviewRating + "/5 stars. Thank you, " + reviewEntry.name + "!";
			reviewSubmitTimer = 5f;
			reviewRating = 0;
			reviewName = "";
			reviewComment = "";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.enabled = true;
		GUI.backgroundColor = guiColorB;
		num2 += 40f;
		if (reviewSubmitTimer > 0f)
		{
			reviewSubmitTimer -= Time.deltaTime;
			GUIStyle val4 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 13,
				fontStyle = (FontStyle)3,
				wordWrap = true,
				richText = true
			};
			val4.normal.textColor = Color.green;
			GUI.Label(new Rect(num, num2, num3, 40f), reviewSubmitResult, val4);
			num2 += 45f;
		}
		if (reviewEntries.Count <= 0)
		{
			return;
		}
		num2 += 5f;
		GUIStyle val5 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 14,
			fontStyle = (FontStyle)1,
			richText = true
		};
		GUI.Label(new Rect(num, num2, num3, 22f), "Past Reviews (" + reviewEntries.Count + ")", val5);
		num2 += 25f;
		float num7 = ((Rect)(ref guiRect)).height - num2 - 5f;
		reviewScrollPosition = GUI.BeginScrollView(new Rect(num, num2, num3, num7), reviewScrollPosition, new Rect(0f, 0f, num3 - 20f, (float)reviewEntries.Count * 60f), false, true);
		for (int j = 0; j < reviewEntries.Count; j++)
		{
			ReviewEntry reviewEntry2 = reviewEntries[j];
			float num8 = (float)j * 60f;
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.2f);
			GUI.Box(new Rect(0f, num8, num3 - 20f, 55f), "");
			string text2 = "";
			for (int k = 0; k < 5; k++)
			{
				text2 += ((k < reviewEntry2.rating) ? "★" : "☆");
			}
			GUIStyle val6 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				fontStyle = (FontStyle)1,
				richText = true
			};
			val6.normal.textColor = guiColorA;
			GUI.Label(new Rect(5f, num8 + 2f, 200f, 18f), reviewEntry2.name, val6);
			GUIStyle val7 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				richText = true
			};
			GUI.Label(new Rect(210f, num8 + 2f, 100f, 18f), text2, val7);
			GUIStyle val8 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10
			};
			val8.normal.textColor = Color.gray;
			GUI.Label(new Rect(num3 - 170f, num8 + 2f, 150f, 18f), reviewEntry2.timestamp, val8);
			if (!string.IsNullOrEmpty(reviewEntry2.comment))
			{
				GUIStyle val9 = new GUIStyle(GUI.skin.label)
				{
					fontSize = 11,
					wordWrap = true
				};
				GUI.Label(new Rect(5f, num8 + 22f, num3 - 30f, 30f), reviewEntry2.comment, val9);
			}
		}
		GUI.EndScrollView();
	}

	private void OnEnable()
	{
		if (PhotonNetwork.NetworkingClient != null)
		{
			PhotonNetwork.NetworkingClient.EventReceived += OnChatEvent;
			PhotonNetwork.NetworkingClient.EventReceived += OnAnnounceEvent;
			PhotonNetwork.NetworkingClient.EventReceived += OnSuggestionEvent;
			PhotonNetwork.NetworkingClient.EventReceived += OnMenuStatusEvent;
		}
	}

	private void OnDisable()
	{
		if (PhotonNetwork.NetworkingClient != null)
		{
			PhotonNetwork.NetworkingClient.EventReceived -= OnChatEvent;
			PhotonNetwork.NetworkingClient.EventReceived -= OnAnnounceEvent;
			PhotonNetwork.NetworkingClient.EventReceived -= OnSuggestionEvent;
		}
	}

	private void Start()
	{
		Instance = this;
		IsOpen = true;
		SyncFromVrTheme();
		lastSyncedThemeType = Main.themeType;
		InitPlayers();
		LoadLocalAnnouncements();
		LoadSuggestions();
		LoadReviews();
		if (PhotonNetwork.NetworkingClient != null)
		{
			PhotonNetwork.NetworkingClient.EventReceived -= OnChatEvent;
			PhotonNetwork.NetworkingClient.EventReceived -= OnAnnounceEvent;
			PhotonNetwork.NetworkingClient.EventReceived -= OnSuggestionEvent;
			PhotonNetwork.NetworkingClient.EventReceived += OnChatEvent;
			PhotonNetwork.NetworkingClient.EventReceived += OnAnnounceEvent;
			PhotonNetwork.NetworkingClient.EventReceived += OnSuggestionEvent;
		}
	}

	private void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		if (!playersInited)
		{
			InitPlayers();
		}
		if (wasdEnabled)
		{
			DoWASD();
		}
		RecordPlayerMacroStep();
		PlayPlayerMacroStep();
		if (isRainbowTheme)
		{
			rainbowTime += Time.deltaTime * 0.5f;
			guiColorA = Color.HSVToRGB(rainbowTime * 0.15f % 1f, 0.9f, 1f);
			guiColorB = Color.HSVToRGB((rainbowTime * 0.15f + 0.5f) % 1f, 0.8f, 1f);
		}
		if (showPlayers && selectedPlayerIndex >= 0 && selectedPlayerIndex < onlinePlayers.Count)
		{
			VRRig selectedPlayerRig = GetSelectedPlayerRig();
			if ((Object)(object)selectedPlayerRig != (Object)null && (Object)(object)selectedPlayerRig.headMesh != (Object)null)
			{
				if ((Object)(object)fpCamera == (Object)null)
				{
					fpCamera = new GameObject("Seralyth_FPCamera").AddComponent<Camera>();
					fpRenderTexture = new RenderTexture(320, 240, 16);
					fpCamera.targetTexture = fpRenderTexture;
					fpCamera.nearClipPlane = 0.01f;
					fpCamera.fieldOfView = 90f;
					fpCamera.clearFlags = (CameraClearFlags)2;
					fpCamera.backgroundColor = Color.black;
				}
				Transform transform = selectedPlayerRig.headMesh.transform;
				Transform transform2 = ((Component)selectedPlayerRig).transform;
				if (camMode == 0)
				{
					((Component)fpCamera).transform.position = transform.position + transform.forward * 0.1f;
					((Component)fpCamera).transform.rotation = transform.rotation;
				}
				else if (camMode == 1)
				{
					Vector3 val = transform2.position - transform.forward * 1.5f + Vector3.up * 0.6f;
					((Component)fpCamera).transform.position = Vector3.Lerp(((Component)fpCamera).transform.position, val, Time.deltaTime * 8f);
					((Component)fpCamera).transform.LookAt(transform.position + Vector3.up * 0.2f);
				}
				else if (camMode == 2)
				{
					Vector3 val2 = transform2.position + transform.forward * 1.5f + Vector3.up * 0.6f;
					((Component)fpCamera).transform.position = Vector3.Lerp(((Component)fpCamera).transform.position, val2, Time.deltaTime * 8f);
					((Component)fpCamera).transform.LookAt(transform.position + Vector3.up * 0.2f);
				}
				else if (camMode == 3 && Time.time >= videoCamTimer)
				{
					Vector3 val3 = Vector3.Cross(transform.forward, Vector3.up);
					Vector3 val4 = ((Vector3)(ref val3)).normalized * ((Random.value > 0.5f) ? 1f : (-1f)) * 2f;
					Vector3 position = transform2.position + val4 + Vector3.up * 0.3f;
					((Component)fpCamera).transform.position = position;
					((Component)fpCamera).transform.LookAt(transform.position + Vector3.up * 0.1f);
					videoCamTimer = Time.time + 1f;
				}
				if (!((Component)fpCamera).gameObject.activeSelf)
				{
					((Component)fpCamera).gameObject.SetActive(true);
				}
			}
		}
		else if ((Object)(object)fpCamera != (Object)null && ((Component)fpCamera).gameObject.activeSelf)
		{
			((Component)fpCamera).gameObject.SetActive(false);
		}
	}

	private VRRig GetSelectedPlayerRig()
	{
		if (selectedPlayerIndex < 0 || selectedPlayerIndex >= onlinePlayers.Count)
		{
			return null;
		}
		string text = onlinePlayers[selectedPlayerIndex];
		if (text.EndsWith("(you)"))
		{
			return VRRig.LocalRig;
		}
		string text2 = text;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!((Object)(object)activeRig == (Object)null) && !activeRig.isLocal)
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(activeRig);
				if (playerFromVRRig != null && playerFromVRRig.NickName == text2)
				{
					return activeRig;
				}
			}
		}
		return null;
	}

	private void CapturePortrait(string pname)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		VRRig selectedPlayerRig = GetSelectedPlayerRig();
		if (!((Object)(object)selectedPlayerRig == (Object)null) && !((Object)(object)selectedPlayerRig.headMesh == (Object)null))
		{
			if ((Object)(object)portraitCamera == (Object)null)
			{
				portraitCamera = new GameObject("Seralyth_PortraitCamera").AddComponent<Camera>();
				portraitRenderTexture = new RenderTexture(320, 240, 16);
				portraitCamera.targetTexture = portraitRenderTexture;
				portraitCamera.nearClipPlane = 0.01f;
				portraitCamera.fieldOfView = 35f;
				portraitCamera.clearFlags = (CameraClearFlags)2;
				portraitCamera.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
			}
			Transform transform = selectedPlayerRig.headMesh.transform;
			Transform transform2 = ((Component)selectedPlayerRig).transform;
			Vector3 position = transform2.position + transform.forward * 1.8f + Vector3.up * 0.5f;
			((Component)portraitCamera).transform.position = position;
			((Component)portraitCamera).transform.LookAt(transform.position + Vector3.up * 0.2f);
			((Component)portraitCamera).gameObject.SetActive(true);
			portraitCamera.Render();
			if ((Object)(object)playerPortrait == (Object)null)
			{
				playerPortrait = new Texture2D(320, 240, (TextureFormat)4, false);
			}
			RenderTexture.active = portraitRenderTexture;
			playerPortrait.ReadPixels(new Rect(0f, 0f, 320f, 240f), 0, 0);
			playerPortrait.Apply();
			RenderTexture.active = null;
			playerPortraitName = pname;
			((Component)portraitCamera).gameObject.SetActive(false);
		}
	}

	private void CaptureSelfPortrait()
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		VRRig localRig = VRRig.LocalRig;
		if (!((Object)(object)localRig == (Object)null) && !((Object)(object)localRig.headMesh == (Object)null))
		{
			if ((Object)(object)portraitCamera == (Object)null)
			{
				portraitCamera = new GameObject("Seralyth_PortraitCamera").AddComponent<Camera>();
				portraitRenderTexture = new RenderTexture(160, 160, 16);
				portraitCamera.targetTexture = portraitRenderTexture;
				portraitCamera.nearClipPlane = 0.01f;
				portraitCamera.fieldOfView = 40f;
				portraitCamera.clearFlags = (CameraClearFlags)2;
				portraitCamera.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
			}
			Transform transform = localRig.headMesh.transform;
			Transform transform2 = ((Component)localRig).transform;
			Vector3 position = transform2.position + transform.forward * 1.6f + Vector3.up * 0.3f;
			((Component)portraitCamera).transform.position = position;
			((Component)portraitCamera).transform.LookAt(transform.position + Vector3.up * 0.15f);
			((Component)portraitCamera).gameObject.SetActive(true);
			portraitCamera.Render();
			if ((Object)(object)selfPortrait == (Object)null)
			{
				selfPortrait = new Texture2D(160, 160, (TextureFormat)4, false);
			}
			RenderTexture.active = portraitRenderTexture;
			selfPortrait.ReadPixels(new Rect(0f, 0f, 160f, 160f), 0, 0);
			selfPortrait.Apply();
			RenderTexture.active = null;
			selfPortraitCaptured = true;
			((Component)portraitCamera).gameObject.SetActive(false);
		}
	}

	private void DrawPlayerPortraitWindow(int id)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		float width = ((Rect)(ref portraitWindowRect)).width;
		float height = ((Rect)(ref portraitWindowRect)).height;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 13,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		GUI.color = guiColorA;
		GUI.Label(new Rect(0f, 4f, width, 20f), "Player Photo", val);
		GUI.color = Color.white;
		GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
		GUI.Box(new Rect(8f, 24f, width - 16f, 1f), "");
		GUI.backgroundColor = Color.clear;
		string text = onlinePlayers[selectedPlayerIndex];
		if ((Object)(object)playerPortrait != (Object)null && playerPortraitName == text)
		{
			float num = width - 16f;
			float num2 = height - 68f;
			GUI.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
			GUI.Box(new Rect(8f, 30f, num, num2), "");
			GUI.DrawTexture(new Rect(8f, 30f, num, num2), (Texture)(object)playerPortrait, (ScaleMode)0);
			GUI.backgroundColor = Color.clear;
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.8f);
			GUI.Box(new Rect(8f, 30f + num2 - 24f, num, 24f), "");
			GUI.backgroundColor = Color.clear;
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(8f, 30f + num2 - 24f, num, 24f), text, val2);
		}
		else
		{
			GUI.color = new Color(0.5f, 0.5f, 0.5f);
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 11,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(0f, height / 2f - 10f, width, 20f), "Click a player to load photo", val3);
			GUI.color = Color.white;
		}
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(width / 2f - 45f, height - 30f, 90f, 22f), "Refresh"))
		{
			CapturePortrait(text);
		}
		GUI.backgroundColor = Color.clear;
		GUI.DragWindow();
	}

	private unsafe void DrawPlayerCosmeticsWindow(int id)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Expected O, but got Unknown
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Expected O, but got Unknown
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Expected O, but got Unknown
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		float width = ((Rect)(ref cosmeticsWindowRect)).width;
		float height = ((Rect)(ref cosmeticsWindowRect)).height;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 13,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		GUI.color = guiColorA;
		GUI.Label(new Rect(0f, 4f, width, 20f), "Cosmetics", val);
		GUI.color = Color.white;
		GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
		GUI.Box(new Rect(8f, 24f, width - 16f, 1f), "");
		GUI.backgroundColor = Color.clear;
		if (!CosmeticsController.hasInstance)
		{
			GUI.color = new Color(0.5f, 0.5f, 0.5f);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(0f, height / 2f - 10f, width, 20f), "Cosmetics not loaded", val2);
			GUI.color = Color.white;
			GUI.DragWindow();
			return;
		}
		VRRig selectedPlayerRig = GetSelectedPlayerRig();
		if ((Object)(object)selectedPlayerRig == (Object)null)
		{
			GUI.color = new Color(0.5f, 0.5f, 0.5f);
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(0f, height / 2f - 10f, width, 20f), "No player data", val3);
			GUI.color = Color.white;
			GUI.DragWindow();
			return;
		}
		HashSet<string> playerOwnedCosmetics = selectedPlayerRig._playerOwnedCosmetics;
		HashSet<string> hashSet = (((Object)(object)VRRig.LocalRig != (Object)null) ? VRRig.LocalRig._playerOwnedCosmetics : new HashSet<string>());
		CosmeticsController instance = CosmeticsController.instance;
		List<CosmeticItem> list = new List<CosmeticItem>();
		foreach (string item in playerOwnedCosmetics)
		{
			CosmeticItem itemFromDict = instance.GetItemFromDict(item);
			if (!itemFromDict.isNullItem)
			{
				list.Add(itemFromDict);
			}
		}
		GUIStyle val4 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 10,
			richText = true,
			wordWrap = true
		};
		GUIStyle val5 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 9,
			richText = true,
			alignment = (TextAnchor)5
		};
		float num = 28f;
		float num2 = (float)list.Count * num;
		float num3 = 28f;
		float num4 = height - num3 - 4f;
		cosmeticsScrollPos = GUI.BeginScrollView(new Rect(0f, num3, width, num4), cosmeticsScrollPos, new Rect(0f, 0f, width - 16f, Mathf.Max(num4, num2)), false, true);
		for (int i = 0; i < list.Count; i++)
		{
			float num5 = (float)i * num;
			CosmeticItem val6 = list[i];
			bool flag = hashSet.Contains(val6.itemName);
			bool flag2 = IsCosmeticWorn(val6.itemName);
			string text = ((object)(*(CosmeticCategory*)(&val6.itemCategory))/*cast due to .constrained prefix*/).ToString();
			string text2 = ((!string.IsNullOrEmpty(val6.overrideDisplayName)) ? val6.overrideDisplayName : val6.displayName);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = val6.itemName;
			}
			GUI.backgroundColor = (flag2 ? new Color(0.2f, 0.4f, 0.2f, 0.5f) : new Color(0.15f, 0.15f, 0.2f, 0.5f));
			GUI.Box(new Rect(0f, num5, width - 16f, num - 2f), "");
			GUI.backgroundColor = Color.clear;
			GUI.color = (flag ? new Color(0.6f, 0.9f, 0.6f) : new Color(0.7f, 0.7f, 0.75f));
			GUI.Label(new Rect(4f, num5 + 2f, width - 80f, 14f), "<size=10>" + text2 + "</size>", val4);
			GUI.color = new Color(0.5f, 0.5f, 0.55f);
			GUI.Label(new Rect(4f, num5 + 14f, width - 80f, 12f), "<size=8>" + text + "</size>", val4);
			if (val6.cost > 0)
			{
				GUI.color = new Color(0.9f, 0.8f, 0.3f);
				GUI.Label(new Rect(width - 78f, num5 + 2f, 60f, 14f), $"<size=9>{val6.cost} SR</size>", val5);
			}
			GUI.color = Color.white;
			if (flag)
			{
				GUI.color = new Color(0.5f, 0.8f, 0.5f);
				GUI.Label(new Rect(width - 50f, num5 + 14f, 40f, 12f), "<size=8>Owned</size>", val5);
				GUI.color = Color.white;
				continue;
			}
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(width - 68f, num5 + 12f, 54f, 16f), "<size=8>Add</size>"))
			{
				Fun.AddCosmeticToCart(val6.itemName);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = Color.clear;
		}
		GUI.EndScrollView();
		GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
		GUI.Box(new Rect(8f, height - 22f, width - 16f, 1f), "");
		GUI.backgroundColor = Color.clear;
		GUI.color = new Color(0.6f, 0.6f, 0.65f);
		GUIStyle val7 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 9,
			alignment = (TextAnchor)4
		};
		GUI.Label(new Rect(0f, height - 18f, width, 14f), $"{list.Count} cosmetics", val7);
		GUI.color = Color.white;
		GUI.DragWindow();
	}

	private void DoWASD()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)GTPlayer.Instance.headCollider).transform;
		float num = wasdSpeed * Time.deltaTime;
		if (Input.GetKey((KeyCode)119))
		{
			Transform transform2 = ((Component)GTPlayer.Instance).transform;
			transform2.position += transform.forward * num;
		}
		if (Input.GetKey((KeyCode)115))
		{
			Transform transform3 = ((Component)GTPlayer.Instance).transform;
			transform3.position += transform.forward * (0f - num);
		}
		if (Input.GetKey((KeyCode)100))
		{
			Transform transform4 = ((Component)GTPlayer.Instance).transform;
			transform4.position += transform.right * num;
		}
		if (Input.GetKey((KeyCode)97))
		{
			Transform transform5 = ((Component)GTPlayer.Instance).transform;
			transform5.position += transform.right * (0f - num);
		}
		if (Input.GetKey((KeyCode)113))
		{
			((Component)GTPlayer.Instance).transform.Rotate(0f, 0f - wasdRotation, 0f);
		}
		if (Input.GetKey((KeyCode)101))
		{
			((Component)GTPlayer.Instance).transform.Rotate(0f, wasdRotation, 0f);
		}
		if (Input.GetKey((KeyCode)306))
		{
			Transform transform6 = ((Component)GTPlayer.Instance).transform;
			transform6.position += transform.up * (0f - num);
		}
		if (Input.GetKey((KeyCode)32))
		{
			Transform transform7 = ((Component)GTPlayer.Instance).transform;
			transform7.position += transform.up * wasdJump * Time.deltaTime;
		}
		((Component)GTPlayer.Instance).GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
	}

	private void DrawMirrorWindow(int id)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		float width = ((Rect)(ref mirrorWindowRect)).width;
		float height = ((Rect)(ref mirrorWindowRect)).height;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 13,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		GUI.color = guiColorA;
		GUI.Label(new Rect(0f, 4f, width, 20f), "Live Mirror", val);
		GUI.color = Color.white;
		GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
		GUI.Box(new Rect(8f, 24f, width - 16f, 1f), "");
		GUI.backgroundColor = Color.clear;
		if ((Object)(object)VRRig.LocalRig == (Object)null || (Object)(object)VRRig.LocalRig.headMesh == (Object)null)
		{
			GUI.color = new Color(0.5f, 0.5f, 0.5f);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(0f, height / 2f - 10f, width, 20f), "Waiting for player...", val2);
			GUI.color = Color.white;
			GUI.DragWindow();
			return;
		}
		if ((Object)(object)mirrorCamera == (Object)null)
		{
			mirrorCamera = new GameObject("Seralyth_MirrorCamera").AddComponent<Camera>();
			mirrorRenderTexture = new RenderTexture(320, 240, 16);
			mirrorCamera.targetTexture = mirrorRenderTexture;
			mirrorCamera.nearClipPlane = 0.01f;
			mirrorCamera.fieldOfView = 60f;
			mirrorCamera.clearFlags = (CameraClearFlags)2;
			mirrorCamera.backgroundColor = new Color(0.06f, 0.06f, 0.1f);
		}
		Transform transform = VRRig.LocalRig.headMesh.transform;
		Transform transform2 = ((Component)VRRig.LocalRig).transform;
		if (!((Component)mirrorCamera).gameObject.activeSelf)
		{
			((Component)mirrorCamera).gameObject.SetActive(true);
		}
		float num = width - 16f;
		float num2 = height - 70f;
		GUI.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
		GUI.Box(new Rect(8f, 30f, num, num2), "");
		GUI.DrawTexture(new Rect(8f, 30f, num, num2), (Texture)(object)mirrorRenderTexture, (ScaleMode)0);
		GUI.backgroundColor = Color.clear;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(width / 2f - 45f, height - 30f, 90f, 22f), "Refresh"))
		{
			Vector3 position = transform2.position + transform.forward * 1.2f + Vector3.up * 0.4f;
			((Component)mirrorCamera).transform.position = position;
			((Component)mirrorCamera).transform.LookAt(transform2.position + Vector3.up * 0.4f);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = Color.clear;
		GUI.DragWindow();
	}

	private void DrawGUISettingsTab()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		float num = 175f;
		float num2 = 50f;
		float num3 = 300f;
		float num4 = 30f;
		float num5 = 38f;
		GUI.Label(new Rect(num, num2, num3, 25f), "<b>GUI Settings</b>");
		num2 += 35f;
		GUI.backgroundColor = (enableRainbowSnake ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num3, num4), enableRainbowSnake ? "Rainbow Border Snake: ON" : "Rainbow Border Snake: OFF"))
		{
			enableRainbowSnake = !enableRainbowSnake;
			if (!enableRainbowSnake)
			{
				snakeTrail.Clear();
			}
			PlayerPrefs.SetInt("GUI_RainbowSnake", enableRainbowSnake ? 1 : 0);
			PlayerPrefs.Save();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num2 += num5;
		GUI.backgroundColor = (enableMouseGlow ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num3, num4), enableMouseGlow ? "Mouse Glow Effect: ON" : "Mouse Glow Effect: OFF"))
		{
			enableMouseGlow = !enableMouseGlow;
			if (!enableMouseGlow)
			{
				glowBlobs.Clear();
			}
			PlayerPrefs.SetInt("GUI_MouseGlow", enableMouseGlow ? 1 : 0);
			PlayerPrefs.Save();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num2 += num5;
		GUI.backgroundColor = ((tooltipStyle != 0) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(num, num2, num3, num4), "Tooltip Style <color=grey>[</color><color=green>" + tooltipStyleNames[tooltipStyle] + "</color><color=grey>]</color>"))
		{
			tooltipStyle = (tooltipStyle + 1) % tooltipStyleNames.Length;
			typewriterTarget = "";
			typewriterChars = 0;
			PlayerPrefs.SetInt("GUI_TooltipStyle", tooltipStyle);
			PlayerPrefs.Save();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num2 += num5;
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num, num2, num3, 25f), "<b>Recently Used</b>");
		num2 += 25f;
		if (Main.recentlyUsed.Count == 0)
		{
			GUI.Label(new Rect(num, num2, num3, 20f), "<color=grey>No mods used yet.</color>");
		}
		else
		{
			for (int i = 0; i < Main.recentlyUsed.Count; i++)
			{
				string text = Main.recentlyUsed[i];
				ButtonInfo index = Buttons.GetIndex(text);
				if (index != null)
				{
					GUI.backgroundColor = (index.enabled ? guiColorA : guiColorB);
					if (GUI.Button(new Rect(num, num2, num3, num4), text))
					{
						Main.Toggle(index);
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					num2 += 28f;
				}
			}
		}
		GUI.backgroundColor = guiColorB;
	}

	private void LoadGameBackgrounds()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		if (gameBgsLoaded)
		{
			return;
		}
		if (!gameBgLoadStarted)
		{
			gameBackgrounds = (Texture2D[])(object)new Texture2D[gameBgPaths.Length];
			gameBgLoadIndex = 0;
			gameBgLoadStarted = true;
		}
		if (gameBgLoadIndex < gameBgPaths.Length)
		{
			try
			{
				if (File.Exists(gameBgPaths[gameBgLoadIndex]))
				{
					byte[] array = File.ReadAllBytes(gameBgPaths[gameBgLoadIndex]);
					gameBackgrounds[gameBgLoadIndex] = new Texture2D(2, 2);
					ImageConversion.LoadImage(gameBackgrounds[gameBgLoadIndex], array);
				}
			}
			catch
			{
				gameBackgrounds[gameBgLoadIndex] = null;
			}
			gameBgLoadIndex++;
		}
		else
		{
			gameBgsLoaded = true;
		}
	}

	private void DrawGameBackground(float x, float y, float w, float h)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			LoadGameBackgrounds();
			if (gameBackgrounds == null || gameBackgrounds.Length == 0)
			{
				return;
			}
			int num = gameMode % gameBackgrounds.Length;
			if (num >= 0 && num < gameBackgrounds.Length)
			{
				Texture2D val = gameBackgrounds[num];
				if (!((Object)(object)val == (Object)null))
				{
					Color color = GUI.color;
					GUI.color = new Color(1f, 1f, 1f, 0.15f);
					GUI.DrawTexture(new Rect(x, y, w, h), (Texture)(object)val, (ScaleMode)0);
					GUI.color = color;
				}
			}
		}
		catch
		{
		}
	}

	private void DrawGamesTab()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 28f, 50f, 20f), "Game:");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(225f, 26f, 30f, 22f), "<"))
		{
			gameMode = (gameMode - 1 + gameNames.Length) % gameNames.Length;
			showGameHelp = false;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = new Color(0.22f, 0.22f, 0.28f);
		GUI.Button(new Rect(260f, 26f, 130f, 22f), gameNames[gameMode]);
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(395f, 26f, 30f, 22f), ">"))
		{
			gameMode = (gameMode + 1) % gameNames.Length;
			showGameHelp = false;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = (Color)(showGameHelp ? new Color(0.6f, 0.4f, 0.1f) : guiColorB);
		if (GUI.Button(new Rect(430f, 26f, 80f, 22f), showGameHelp ? "Close Help" : "? How to Play"))
		{
			showGameHelp = !showGameHelp;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (showGameHelp)
		{
			GUI.backgroundColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
			GUI.Box(new Rect(185f, 52f, 500f, 100f), "");
			GUI.backgroundColor = guiColorB;
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 13,
				wordWrap = true,
				richText = true,
				alignment = (TextAnchor)0
			};
			GUI.Label(new Rect(195f, 58f, 480f, 88f), "<b>" + gameNames[gameMode] + "</b>\n" + gameHelp[gameMode], val);
			DrawGameBackground(170f, 158f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 158f);
		}
		else
		{
			DrawGameBackground(170f, 50f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 50f);
		}
		float num = (showGameHelp ? 158f : 50f);
		float num2 = ((Rect)(ref guiRect)).height - num;
		gameScrollPosition = GUI.BeginScrollView(new Rect(170f, num, ((Rect)(ref guiRect)).width - 170f, num2), gameScrollPosition, new Rect(170f, 0f, ((Rect)(ref guiRect)).width - 170f, 800f), false, true);
		if (gameMode == 0)
		{
			DrawTicTacToe();
		}
		else if (gameMode == 1)
		{
			DrawWordleTab();
		}
		else if (gameMode == 2)
		{
			DrawBlockBlast();
		}
		else if (gameMode == 3)
		{
			DrawSnake();
		}
		else if (gameMode == 4)
		{
			DrawConnectFour();
		}
		else if (gameMode == 5)
		{
			DrawFlappyBird();
		}
		else if (gameMode == 6)
		{
			DrawMinesweeper();
		}
		else if (gameMode == 7)
		{
			Draw2048();
		}
		else if (gameMode == 8)
		{
			DrawPong();
		}
		else if (gameMode == 9)
		{
			DrawSimon();
		}
		else if (gameMode == 10)
		{
			DrawHangman();
		}
		else if (gameMode == 11)
		{
			DrawMemory();
		}
		else if (gameMode == 12)
		{
			DrawCheckers();
		}
		else if (gameMode == 13)
		{
			DrawSudoku();
		}
		else if (gameMode == 14)
		{
			DrawTowerDefense();
		}
		else if (gameMode == 15)
		{
			DrawMaze();
		}
		else if (gameMode == 16)
		{
			DrawBreakout();
		}
		else if (gameMode == 17)
		{
			DrawMSHard();
		}
		else if (gameMode == 18)
		{
			DrawChineseCheckers();
		}
		else if (gameMode == 19)
		{
			DrawTetris();
		}
		else if (gameMode == 20)
		{
			DrawSolitaire();
		}
		else if (gameMode == 21)
		{
			DrawChess();
		}
		else if (gameMode == 22)
		{
			DrawWhackAMole();
		}
		else if (gameMode == 23)
		{
			DrawReactionTest();
		}
		else if (gameMode == 24)
		{
			DrawTypingSpeed();
		}
		else if (gameMode == 25)
		{
			DrawCatchObjects();
		}
		else if (gameMode == 26)
		{
			DrawPacman();
		}
		else if (gameMode == 27)
		{
			DrawTankBattle();
		}
		else if (gameMode == 28)
		{
			DrawBattleship();
		}
		else if (gameMode == 29)
		{
			DrawYahtzee();
		}
		else if (gameMode == 30)
		{
			DrawColorMatch();
		}
		else if (gameMode == 31)
		{
			DrawPipePuzzle();
		}
		else if (gameMode == 32)
		{
			DrawLightsOut();
		}
		else if (gameMode == 33)
		{
			DrawNonogram();
		}
		else if (gameMode == 34)
		{
			DrawRockPaperScissors();
		}
		else if (gameMode == 35)
		{
			DrawNumberGuess();
		}
		else if (gameMode == 36)
		{
			DrawDiceRoll();
		}
		else if (gameMode == 37)
		{
			DrawCoinFlip();
		}
		else if (gameMode == 38)
		{
			DrawBlackjack();
		}
		else if (gameMode == 39)
		{
			DrawGomoku();
		}
		else if (gameMode == 40)
		{
			DrawDotsAndBoxes();
		}
		else if (gameMode == 41)
		{
			DrawCheckers2P();
		}
		else if (gameMode == 42)
		{
			DrawSlidingPuzzle();
		}
		else if (gameMode == 43)
		{
			DrawBullsAndCows();
		}
		else if (gameMode == 44)
		{
			DrawFreeCell();
		}
		else if (gameMode == 45)
		{
			DrawTron();
		}
		else if (gameMode == 46)
		{
			DrawBomberman();
		}
		else if (gameMode == 47)
		{
			DrawBrickCalculator();
		}
		else if (gameMode == 48)
		{
			DrawOthello();
		}
		else
		{
			DrawRushHour();
		}
		GUI.EndScrollView();
	}

	private void DrawTicTacToe()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Expected O, but got Unknown
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(170f, 52f, 300f, 30f), "<size=18>Tic Tac Toe</size>");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(450f, 46f, 80f, 22f), "New Game"))
		{
			ResetTT();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		string text = (ttPlayerIsX ? "X" : "O");
		string text2 = (ttPlayerIsX ? "O" : "X");
		GUI.Label(new Rect(170f, 75f, 200f, 20f), "You: " + text + "  |  AI: " + text2);
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(380f, 73f, 80f, 22f), "Switch"))
		{
			ttPlayerIsX = !ttPlayerIsX;
			ResetTT();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			if (!ttPlayerIsX)
			{
				ttTurn = 1;
				ttAICooldown = Time.time + 0.4f;
				((MonoBehaviour)this).Invoke("TTAIMove", 0.35f);
			}
		}
		string[] array = new string[3] { "Easy", "Normal", "Hard" };
		GUI.Label(new Rect(170f, 97f, 60f, 20f), "Difficulty:");
		GUI.backgroundColor = ((ttDiff == 0) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(240f, 97f, 55f, 20f), array[0]))
		{
			ttDiff = 0;
			ResetTT();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = ((ttDiff == 1) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(300f, 97f, 55f, 20f), array[1]))
		{
			ttDiff = 1;
			ResetTT();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = ((ttDiff == 2) ? guiColorA : guiColorB);
		if (GUI.Button(new Rect(360f, 97f, 55f, 20f), array[2]))
		{
			ttDiff = 2;
			ResetTT();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 122f, 200f, 20f), $"Wins: {ttScoreX}  |  Losses: {ttScoreO}  |  Draws: {ttScoreD}");
		float num = 80f;
		float num2 = 6f;
		float num3 = 230f;
		float num4 = 150f;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int num5 = i * 3 + j;
				float num6 = num3 + (float)j * (num + num2);
				float num7 = num4 + (float)i * (num + num2);
				bool flag = false;
				if (ttWinner == 1 || ttWinner == 2)
				{
					int[,] array2 = new int[8, 3]
					{
						{ 0, 1, 2 },
						{ 3, 4, 5 },
						{ 6, 7, 8 },
						{ 0, 3, 6 },
						{ 1, 4, 7 },
						{ 2, 5, 8 },
						{ 0, 4, 8 },
						{ 2, 4, 6 }
					};
					for (int k = 0; k < 8; k++)
					{
						int num8 = array2[k, 0];
						int num9 = array2[k, 1];
						int num10 = array2[k, 2];
						if ((num8 == num5 || num9 == num5 || num10 == num5) && ttb[num8] == ttb[num9] && ttb[num9] == ttb[num10] && ttb[num8] != "")
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
				}
				else if (ttb[num5] == "X")
				{
					GUI.backgroundColor = new Color(0.3f, 0.5f, 1f);
				}
				else if (ttb[num5] == "O")
				{
					GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
				}
				else
				{
					GUI.backgroundColor = new Color(0.25f, 0.25f, 0.3f);
				}
				string text3 = ((ttb[num5] == "") ? " " : ttb[num5]);
				GUIStyle val = new GUIStyle(GUI.skin.button)
				{
					fontSize = 32,
					fontStyle = (FontStyle)1
				};
				if (GUI.Button(new Rect(num6, num7, num, num), text3, val) && ttb[num5] == "" && ttWinner == 0 && ttTurn == 0 && Time.time > ttAICooldown)
				{
					ttb[num5] = ttPlayerSym;
					ttTurn = 1;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					CheckTTWin();
					if (ttWinner == 0 && !ttFull())
					{
						ttAICooldown = Time.time + 0.4f;
						((MonoBehaviour)this).Invoke("TTAIMove", 0.35f);
					}
				}
			}
		}
		if (ttLineA >= 0 && ttLineB >= 0)
		{
			int num11 = ttLineA / 3;
			int num12 = ttLineA % 3;
			int num13 = ttLineB / 3;
			int num14 = ttLineB % 3;
			float num15 = num3 + (float)num12 * (num + num2) + num * 0.5f;
			float num16 = num4 + (float)num11 * (num + num2) + num * 0.5f;
			float num17 = num3 + (float)num14 * (num + num2) + num * 0.5f;
			float num18 = num4 + (float)num13 * (num + num2) + num * 0.5f;
			Color color = (((ttWinner == 1 && ttPlayerIsX) || (ttWinner == 2 && !ttPlayerIsX)) ? new Color(0.3f, 0.8f, 1f) : new Color(1f, 0.5f, 0.2f));
			float num19 = 5f;
			int num20 = ((!(Mathf.Max(Mathf.Abs(num17 - num15), Mathf.Abs(num18 - num16)) > 0f)) ? 1 : ((int)Mathf.Max(Mathf.Abs(num17 - num15), Mathf.Abs(num18 - num16))));
			for (int l = 0; l <= num20; l++)
			{
				float num21 = ((num20 == 0) ? 0f : ((float)l / (float)num20));
				float num22 = Mathf.Lerp(num15, num17, num21);
				float num23 = Mathf.Lerp(num16, num18, num21);
				GUI.color = color;
				GUI.DrawTexture(new Rect(num22 - num19 * 0.5f, num23 - num19 * 0.5f, num19, num19), (Texture)(object)Texture2D.whiteTexture);
			}
			GUI.color = Color.white;
		}
		GUI.backgroundColor = guiColorB;
		string text4 = ((ttWinner == 1 || ttWinner == 2) ? (((ttWinner == 1 && ttPlayerIsX) || (ttWinner == 2 && !ttPlayerIsX)) ? "You win!" : "AI wins!") : ((!ttFull()) ? ((ttTurn == 0) ? "Your turn" : "AI thinking...") : "Draw!"));
		GUI.Label(new Rect(170f, 410f, 300f, 25f), "<size=14>" + text4 + "</size>");
		GUI.backgroundColor = guiColorB;
	}

	private void DrawWordleTab()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Expected O, but got Unknown
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Expected O, but got Unknown
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c48: Unknown result type (might be due to invalid IL or missing references)
		if (wdTarget == "")
		{
			wdTarget = GenerateRandomWord().ToUpper();
			wdRow = 0;
			wdHintsUsed = 0;
			wdHintText = null;
			wdUsedHintIndices.Clear();
			for (int i = 0; i < 6; i++)
			{
				wdGuesses[i] = "";
			}
			for (int j = 0; j < 6; j++)
			{
				for (int k = 0; k < 5; k++)
				{
					wdColors[j, k] = 0;
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Wordle</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), "Guess the 5-letter word (6 tries)");
		bool flag = false;
		bool flag2 = false;
		for (int l = 0; l < 6; l++)
		{
			bool flag3 = true;
			for (int m = 0; m < 5; m++)
			{
				float num = 210f + (float)m * 48f;
				float num2 = 80f + (float)l * 48f;
				if (wdColors[l, m] == 1)
				{
					GUI.backgroundColor = new Color(0.1f, 0.9f, 0.2f);
				}
				else if (wdColors[l, m] == 2)
				{
					GUI.backgroundColor = new Color(1f, 0.85f, 0.1f);
				}
				else if (wdColors[l, m] == 3)
				{
					GUI.backgroundColor = new Color(0.5f, 0.5f, 0.55f);
				}
				else if (l < wdRow)
				{
					GUI.backgroundColor = new Color(0.5f, 0.5f, 0.55f);
				}
				else
				{
					GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
				}
				string text = ((wdGuesses[l].Length > m) ? wdGuesses[l][m].ToString() : "");
				GUIStyle val = new GUIStyle(GUI.skin.button)
				{
					fontSize = 24,
					fontStyle = (FontStyle)1
				};
				GUI.Button(new Rect(num, num2, 42f, 42f), text, val);
				if (wdColors[l, m] != 1 && l < wdRow)
				{
					flag3 = false;
				}
			}
			if (l < wdRow && flag3)
			{
				flag = true;
			}
		}
		if (wdRow >= 6 && !flag)
		{
			flag2 = true;
		}
		GUI.backgroundColor = guiColorB;
		if (flag)
		{
			if (!wdResultCounted)
			{
				wdGuessesWon++;
				wdResultCounted = true;
			}
			GUI.Label(new Rect(170f, 380f, 300f, 25f), "<size=14><color=green>You got it!</color>  Word: " + wdTarget + "</size>");
		}
		else if (flag2)
		{
			if (!wdResultCounted)
			{
				wdGuessesLost++;
				wdResultCounted = true;
			}
			GUI.Label(new Rect(170f, 380f, 300f, 25f), "<size=14><color=red>Out of tries!</color>  Word: " + wdTarget + "</size>");
		}
		else if (wdRow < 6)
		{
			GUIStyle val2 = new GUIStyle(GUI.skin.textField)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			wdInput = GUI.TextField(new Rect(210f, 380f, 234f, 30f), wdInput.ToUpper(), 5, val2);
			wdInput = Regex.Replace(wdInput, "[^A-Za-z]", "");
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(455f, 380f, 55f, 30f), "Enter") && wdInput.Length == 5 && Time.time > ttAICooldown)
			{
				string text2 = wdInput.ToUpper();
				wdGuesses[wdRow] = text2;
				string text3 = wdTarget;
				char[] array = text3.ToCharArray();
				char[] array2 = text2.ToCharArray();
				int[] array3 = new int[5];
				bool[] array4 = new bool[5];
				bool[] array5 = new bool[5];
				for (int n = 0; n < 5; n++)
				{
					if (array2[n] == array[n])
					{
						array3[n] = 1;
						array4[n] = true;
						array5[n] = true;
					}
				}
				for (int num3 = 0; num3 < 5; num3++)
				{
					if (array5[num3])
					{
						continue;
					}
					for (int num4 = 0; num4 < 5; num4++)
					{
						if (!array4[num4] && array2[num3] == array[num4])
						{
							array3[num3] = 2;
							array4[num4] = true;
							break;
						}
					}
				}
				for (int num5 = 0; num5 < 5; num5++)
				{
					wdColors[wdRow, num5] = ((array3[num5] == 0) ? 3 : array3[num5]);
				}
				wdRow++;
				wdInput = "";
				ttAICooldown = Time.time + 0.3f;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(450f, 46f, 80f, 22f), "New Word"))
		{
			wdTarget = "";
			wdInput = "";
			wdRow = 0;
			wdHintsUsed = 0;
			wdHintText = null;
			wdUsedHintIndices.Clear();
			wdResultCounted = false;
			for (int num6 = 0; num6 < 6; num6++)
			{
				wdGuesses[num6] = "";
			}
			for (int num7 = 0; num7 < 6; num7++)
			{
				for (int num8 = 0; num8 < 5; num8++)
				{
					wdColors[num7, num8] = 0;
				}
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		bool flag4 = !flag && !flag2 && wdRow < 6 && wdHintsUsed < 2 && wdTarget != "";
		GUI.backgroundColor = (Color)(flag4 ? guiColorA : new Color(0.4f, 0.4f, 0.4f));
		if (flag4 && GUI.Button(new Rect(450f, 70f, 80f, 22f), $"Hint ({2 - wdHintsUsed})"))
		{
			string text4 = "AEIOU";
			int num9 = 0;
			string text5 = wdTarget;
			foreach (char value in text5)
			{
				if (text4.Contains(value))
				{
					num9++;
				}
			}
			bool flag5 = false;
			for (int num11 = 0; num11 < 4; num11++)
			{
				if (wdTarget[num11] == wdTarget[num11 + 1])
				{
					flag5 = true;
				}
			}
			int num12 = 5 - num9;
			int num13 = 0;
			int num14 = 0;
			string text6 = wdTarget;
			foreach (char c in text6)
			{
				if (!text4.Contains(c))
				{
					if (c <= 'M')
					{
						num13++;
					}
					else
					{
						num14++;
					}
				}
			}
			bool flag6 = wdTarget.Distinct().Count() == 5;
			int num16 = 0;
			for (int num17 = 0; num17 < 5; num17++)
			{
				if (text4.Contains(wdTarget[num17]))
				{
					num16 += num17 + 1;
				}
			}
			bool flag7 = true;
			for (int num18 = 0; num18 < 4; num18++)
			{
				if (text4.Contains(wdTarget[num18]) == text4.Contains(wdTarget[num18 + 1]))
				{
					flag7 = false;
					break;
				}
			}
			bool flag8 = text4.Contains(wdTarget[0]);
			bool flag9 = text4.Contains(wdTarget[4]);
			int num19 = 0;
			int num20 = 0;
			for (int num21 = 0; num21 < 5; num21++)
			{
				if (!text4.Contains(wdTarget[num21]))
				{
					num19++;
					if (num19 > num20)
					{
						num20 = num19;
					}
				}
				else
				{
					num19 = 0;
				}
			}
			string[] array6 = new string[12]
			{
				$"The word starts with '<color=green>{wdTarget[0]}</color>'",
				$"The word ends with '<color=green>{wdTarget[4]}</color>'",
				string.Format("The word has <color=yellow>{0}</color> vowel{1} and <color=yellow>{2}</color> consonant{3}", num9, (num9 != 1) ? "s" : "", num12, (num12 != 1) ? "s" : ""),
				$"The middle letter is '<color=green>{wdTarget[2]}</color>'",
				flag5 ? "The word has a <color=yellow>double letter</color>" : "The word has <color=yellow>no double letters</color>",
				flag6 ? "Every letter in the word is <color=yellow>different</color>" : "The word has <color=yellow>repeating letters</color>",
				flag7 ? "The word <color=yellow>alternates</color> vowel and consonant" : "The word has <color=yellow>consecutive</color> vowels or consonants",
				flag8 ? "The word <color=yellow>starts with a vowel</color>" : "The word <color=yellow>starts with a consonant</color>",
				flag9 ? "The word <color=yellow>ends with a vowel</color>" : "The word <color=yellow>ends with a consonant</color>",
				(num20 >= 3) ? "The word has <color=yellow>3+ consonants in a row</color>" : "The word has <color=yellow>at most 2 consonants in a row</color>",
				(num13 > num14) ? "Most consonants are in the <color=yellow>first half</color> of the alphabet" : "Most consonants are in the <color=yellow>second half</color> of the alphabet",
				(num16 <= 9) ? "The vowels sit more toward the <color=yellow>left</color> of the word" : "The vowels sit more toward the <color=yellow>right</color> of the word"
			};
			List<int> list = new List<int>();
			for (int num22 = 0; num22 < array6.Length; num22++)
			{
				if (!wdUsedHintIndices.Contains(num22))
				{
					list.Add(num22);
				}
			}
			if (list.Count == 0)
			{
				wdUsedHintIndices.Clear();
				for (int num23 = 0; num23 < array6.Length; num23++)
				{
					list.Add(num23);
				}
			}
			int num24 = list[Random.Range(0, list.Count)];
			wdUsedHintIndices.Add(num24);
			wdHintText = array6[num24];
			wdHintsUsed++;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (!string.IsNullOrEmpty(wdHintText))
		{
			GUI.Label(new Rect(455f, 75f, 230f, 20f), "<size=11>" + wdHintText + "</size>");
		}
		GUI.Label(new Rect(455f, 92f, 230f, 30f), "<size=9><color=green>Green</color>=right spot  <color=yellow>Yellow</color>=wrong spot  <color=#888899>Grey</color>=not in word</size>");
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 445f, 300f, 20f), $"Won: {wdGuessesWon}  |  Lost: {wdGuessesLost}");
	}

	private void DrawBlockBlast()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Invalid comparison between Unknown and I4
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		if (!bbGameActive && !bbGameOver)
		{
			BBNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Block Blast</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {bbScore}  Best: {bbBestScore}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(450f, 46f, 80f, 22f), "New Game"))
		{
			BBNewGame();
			bbDragging = false;
			bbDragPiece = -1;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		bbScrollPosition = GUI.BeginScrollView(new Rect(170f, 68f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), bbScrollPosition, new Rect(0f, 0f, 500f, 560f), false, true);
		float num = 20f;
		float num2 = 10f;
		float num3 = 35f;
		if ((int)Event.current.type == 0 && Event.current.button == 1 && bbDragging)
		{
			bbDragging = false;
			bbDragPiece = -1;
			Event.current.Use();
		}
		if ((int)Event.current.type == 1 && Event.current.button == 0 && bbDragging && bbDragPiece >= 0)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			int num4 = (int)((mousePosition.x - num) / num3);
			int num5 = (int)((mousePosition.y - num2) / num3);
			if (num5 >= 0 && num5 < 8 && num4 >= 0 && num4 < 8 && BBCanPlace(bbShapeTypes[bbDragPiece], num5, num4))
			{
				BBPlaceBlock(bbDragPiece, num5, num4);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			bbDragging = false;
			bbDragPiece = -1;
			Event.current.Use();
		}
		GUI.backgroundColor = new Color(0.12f, 0.12f, 0.16f);
		GUI.Box(new Rect(num - 3f, num2 - 3f, num3 * 8f + 6f, num3 * 8f + 6f), "");
		Color backgroundColor = default(Color);
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				float num6 = num + (float)j * num3;
				float num7 = num2 + (float)i * num3;
				((Color)(ref backgroundColor))._002Ector(0.18f, 0.18f, 0.22f, 0.9f);
				if (bbGrid[i, j] > 0)
				{
					backgroundColor = bbBlockColors[(bbGrid[i, j] - 1) % bbBlockColors.Length];
				}
				GUI.backgroundColor = backgroundColor;
				GUI.Button(new Rect(num6 + 0.5f, num7 + 0.5f, num3 - 1f, num3 - 1f), "");
			}
		}
		if (bbDragging && bbDragPiece >= 0 && bbGameActive)
		{
			Vector2 mousePosition2 = Event.current.mousePosition;
			int num8 = (int)((mousePosition2.x - num) / num3);
			int num9 = (int)((mousePosition2.y - num2) / num3);
			Vector2Int[] array = bbShapeDefs[bbShapeTypes[bbDragPiece]];
			Color backgroundColor2 = (Color)(BBCanPlace(bbShapeTypes[bbDragPiece], num9, num8) ? bbBlockColors[bbShapeColors[bbDragPiece] % bbBlockColors.Length] : new Color(0.9f, 0.2f, 0.2f, 0.7f));
			for (int k = 0; k < array.Length; k++)
			{
				int num10 = num9 + ((Vector2Int)(ref array[k])).y;
				int num11 = num8 + ((Vector2Int)(ref array[k])).x;
				if (num10 >= 0 && num10 < 8 && num11 >= 0 && num11 < 8)
				{
					float num12 = num + (float)num11 * num3;
					float num13 = num2 + (float)num10 * num3;
					GUI.backgroundColor = backgroundColor2;
					GUI.Button(new Rect(num12 + 0.5f, num13 + 0.5f, num3 - 1f, num3 - 1f), "");
				}
			}
		}
		float num14 = num2 + num3 * 8f + 12f;
		float num15 = 100f;
		float num16 = 80f;
		float num17 = num15 * 3f + 20f;
		float num18 = num + (num3 * 8f - num17) / 2f;
		Rect val = default(Rect);
		for (int l = 0; l < 3; l++)
		{
			float num19 = num18 + (float)l * (num15 + 10f);
			if (bbShapePlaced[l])
			{
				GUI.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.4f);
				GUI.Box(new Rect(num19, num14, num15, num16), "");
				continue;
			}
			GUI.backgroundColor = ((bbDragging && bbDragPiece == l) ? new Color(0.3f, 0.3f, 0.35f, 0.5f) : new Color(0.22f, 0.22f, 0.28f, 0.9f));
			GUI.Box(new Rect(num19, num14, num15, num16), "");
			((Rect)(ref val))._002Ector(num19, num14, num15, num16);
			if ((int)Event.current.type == 0 && Event.current.button == 0 && ((Rect)(ref val)).Contains(Event.current.mousePosition) && bbGameActive)
			{
				bbDragging = true;
				bbDragPiece = l;
				Event.current.Use();
			}
			Vector2Int[] array2 = bbShapeDefs[bbShapeTypes[l]];
			int num20 = 0;
			int num21 = 0;
			Vector2Int[] array3 = array2;
			for (int m = 0; m < array3.Length; m++)
			{
				Vector2Int val2 = array3[m];
				if (((Vector2Int)(ref val2)).y > num20)
				{
					num20 = ((Vector2Int)(ref val2)).y;
				}
				if (((Vector2Int)(ref val2)).x > num21)
				{
					num21 = ((Vector2Int)(ref val2)).x;
				}
			}
			float num22 = 18f;
			float num23 = num19 + (num15 - (float)(num21 + 1) * num22) / 2f;
			float num24 = num14 + (num16 - (float)(num20 + 1) * num22) / 2f;
			Color backgroundColor3 = bbBlockColors[bbShapeColors[l] % bbBlockColors.Length];
			if (bbDragging && bbDragPiece == l)
			{
				backgroundColor3.a = 0.4f;
			}
			Vector2Int[] array4 = array2;
			for (int n = 0; n < array4.Length; n++)
			{
				Vector2Int val3 = array4[n];
				GUI.backgroundColor = backgroundColor3;
				GUI.Button(new Rect(num23 + (float)((Vector2Int)(ref val3)).x * num22, num24 + (float)((Vector2Int)(ref val3)).y * num22, num22 - 1f, num22 - 1f), "");
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num, num14 + num16 + 8f, num3 * 8f, 20f), "<size=10>Drag blocks onto the grid to place them</size>");
		if (bbComboTextTime > 0f && Time.time - bbComboTextTime < 1.5f)
		{
			GUI.color = Color.yellow;
			GUI.backgroundColor = Color.clear;
			GUI.Label(new Rect(num, num2 + num3 * 3.5f, num3 * 8f, 30f), "<size=18><b>" + bbComboText + "</b></size>");
			GUI.color = Color.white;
		}
		if (bbPopupTime > 0f && Time.time - bbPopupTime < 1f)
		{
			float num25 = (Time.time - bbPopupTime) / 1f;
			GUI.color = new Color(1f, 1f, 0.3f, 1f - num25);
			GUI.backgroundColor = Color.clear;
			GUI.Label(new Rect(bbPopupX, bbPopupY - num25 * 30f, 100f, 30f), $"+{bbPopupScore}");
			GUI.color = Color.white;
		}
		GUI.backgroundColor = guiColorB;
		if (!bbGameActive)
		{
			float num26 = num2 + num3 * 3f;
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.75f);
			GUI.Box(new Rect(num - 5f, num26 - 5f, num3 * 8f + 10f, 130f), "");
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.white;
			GUI.Label(new Rect(num, num26, num3 * 8f, 25f), "<size=14><b>Looks like you lost!</b></size>");
			GUI.Label(new Rect(num, num26 + 22f, num3 * 8f, 25f), "<size=13>Want to try again?</size>");
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(num + num3 * 2f, num26 + 55f, 100f, 28f), "Yes"))
			{
				BBNewGame();
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
			if (GUI.Button(new Rect(num + num3 * 2f + 110f, num26 + 55f, 100f, 28f), "No"))
			{
				bbGameActive = false;
				bbScore = 0;
				bbGameOver = false;
			}
			GUI.color = Color.white;
		}
		GUI.EndScrollView();
	}

	private void BBNewGame()
	{
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				bbGrid[i, j] = 0;
			}
		}
		bbScore = 0;
		bbSelectedShape = -1;
		bbGameActive = true;
		bbGameOver = false;
		bbComboTextTime = 0f;
		bbPopupTime = 0f;
		bbClearAnimTime = 0f;
		BBGenerateShapes();
	}

	private void BBGenerateShapes()
	{
		for (int i = 0; i < 3; i++)
		{
			bbShapeTypes[i] = Random.Range(0, bbShapeDefs.Length);
			bbShapeColors[i] = Random.Range(0, bbBlockColors.Length);
			bbShapePlaced[i] = false;
		}
		bbSelectedShape = -1;
	}

	private bool BBCanPlace(int shapeIdx, int row, int col)
	{
		Vector2Int[] array = bbShapeDefs[shapeIdx];
		for (int i = 0; i < array.Length; i++)
		{
			int num = row + ((Vector2Int)(ref array[i])).y;
			int num2 = col + ((Vector2Int)(ref array[i])).x;
			if (num < 0 || num >= 8 || num2 < 0 || num2 >= 8)
			{
				return false;
			}
			if (bbGrid[num, num2] != 0)
			{
				return false;
			}
		}
		return true;
	}

	private void BBPlaceBlock(int pieceIdx, int row, int col)
	{
		Vector2Int[] array = bbShapeDefs[bbShapeTypes[pieceIdx]];
		int num = bbShapeColors[pieceIdx] + 1;
		for (int i = 0; i < array.Length; i++)
		{
			int num2 = row + ((Vector2Int)(ref array[i])).y;
			int num3 = col + ((Vector2Int)(ref array[i])).x;
			bbGrid[num2, num3] = num;
		}
		bbShapePlaced[pieceIdx] = true;
		bbSelectedShape = -1;
		BBClearLines();
		if (BBAllPlaced())
		{
			BBGenerateShapes();
		}
		if (!BBCanPlaceAny())
		{
			bbGameActive = false;
			bbGameOver = true;
			if (bbScore > bbBestScore)
			{
				bbBestScore = bbScore;
			}
		}
	}

	private bool BBAllPlaced()
	{
		for (int i = 0; i < 3; i++)
		{
			if (!bbShapePlaced[i])
			{
				return false;
			}
		}
		return true;
	}

	private bool BBCanPlaceAny()
	{
		for (int i = 0; i < 3; i++)
		{
			if (bbShapePlaced[i])
			{
				continue;
			}
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					if (BBCanPlace(bbShapeTypes[i], j, k))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void BBClearLines()
	{
		HashSet<string> hashSet = new HashSet<string>();
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			bool flag = true;
			for (int j = 0; j < 8; j++)
			{
				if (bbGrid[i, j] == 0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				for (int k = 0; k < 8; k++)
				{
					hashSet.Add(i + "," + k);
				}
				num++;
			}
		}
		for (int l = 0; l < 8; l++)
		{
			bool flag2 = true;
			for (int m = 0; m < 8; m++)
			{
				if (bbGrid[m, l] == 0)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				for (int n = 0; n < 8; n++)
				{
					hashSet.Add(n + "," + l);
				}
				num++;
			}
		}
		if (num > 0)
		{
			foreach (string item in hashSet)
			{
				string[] array = item.Split(',');
				bbGrid[int.Parse(array[0]), int.Parse(array[1])] = 0;
			}
			int num2 = num * num * 10;
			bbScore += num2;
			float num3 = 35f;
			bbPopupTime = Time.time;
			bbPopupScore = num2;
			bbPopupX = 20f + num3 * 3f;
			bbPopupY = 10f + num3 * 3f;
			bbComboTextTime = Time.time;
			if (num >= 4)
			{
				bbComboText = "Quadra!!";
			}
			else if (num >= 3)
			{
				bbComboText = "Triple!!";
			}
			else if (num >= 2)
			{
				bbComboText = "Double!!";
			}
			else
			{
				bbComboText = "";
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		bbClearAnimTime = ((num > 0) ? Time.time : 0f);
	}

	private void DrawSnake()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Invalid comparison between Unknown and I4
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Invalid comparison between Unknown and I4
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Invalid comparison between Unknown and I4
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Invalid comparison between Unknown and I4
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Invalid comparison between Unknown and I4
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Invalid comparison between Unknown and I4
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Invalid comparison between Unknown and I4
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Invalid comparison between Unknown and I4
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Invalid comparison between Unknown and I4
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		if (snakeGrid == null || (!snakeGameActive && snakeScore == 0))
		{
			SnakeNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Snake</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {snakeScore}  Best: {snakeBestScore}  Length: {snakeBody.Count}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(450f, 46f, 80f, 22f), "New Game"))
		{
			SnakeNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		string text = (snakeUseAI ? "AI: ON" : "AI: OFF");
		GUI.backgroundColor = (snakeUseAI ? new Color(0.2f, 0.7f, 0.3f) : new Color(0.5f, 0.5f, 0.5f));
		if (GUI.Button(new Rect(540f, 46f, 60f, 22f), text))
		{
			snakeUseAI = !snakeUseAI;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		snakeScrollPos = GUI.BeginScrollView(new Rect(170f, 68f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), snakeScrollPos, new Rect(0f, 0f, 500f, 560f), false, true);
		float num = 20f;
		float num2 = 60f;
		float num3 = 10f;
		if (snakeAlive && snakeGameActive)
		{
			if (snakeUseAI)
			{
				SnakeAIStep();
			}
			else if ((int)Event.current.type == 4)
			{
				KeyCode keyCode = Event.current.keyCode;
				if ((int)keyCode == 273 || (int)keyCode == 119)
				{
					if (snakeDir != new Vector2Int(0, 1))
					{
						snakeDir = new Vector2Int(0, -1);
					}
				}
				else if ((int)keyCode == 274 || (int)keyCode == 115)
				{
					if (snakeDir != new Vector2Int(0, -1))
					{
						snakeDir = new Vector2Int(0, 1);
					}
				}
				else if ((int)keyCode == 276 || (int)keyCode == 97)
				{
					if (snakeDir != new Vector2Int(1, 0))
					{
						snakeDir = new Vector2Int(-1, 0);
					}
				}
				else if (((int)keyCode == 275 || (int)keyCode == 100) && snakeDir != new Vector2Int(-1, 0))
				{
					snakeDir = new Vector2Int(1, 0);
				}
			}
			snakeMoveTimer += Time.deltaTime;
			if (snakeMoveTimer >= snakeMoveInterval)
			{
				snakeMoveTimer -= snakeMoveInterval;
				SnakeStep();
			}
		}
		GUI.backgroundColor = new Color(0.1f, 0.1f, 0.12f);
		GUI.Box(new Rect(num2 - 3f, num3 - 3f, num * 20f + 6f, num * 15f + 6f), "");
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				GUI.backgroundColor = new Color(0.16f, 0.16f, 0.19f, 0.9f);
				GUI.Button(new Rect(num4 + 0.3f, num5 + 0.3f, num - 0.6f, num - 0.6f), "");
			}
		}
		GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
		float num6 = num2 + (float)((Vector2Int)(ref snakeFood)).x * num;
		float num7 = num3 + (float)((Vector2Int)(ref snakeFood)).y * num;
		GUI.Button(new Rect(num6 + 1f, num7 + 1f, num - 2f, num - 2f), "");
		for (int num8 = snakeBody.Count - 1; num8 >= 0; num8--)
		{
			float num9 = (float)num8 / (float)Mathf.Max(1, snakeBody.Count - 1);
			if (num8 == 0)
			{
				GUI.backgroundColor = new Color(0.2f, 0.9f, 0.3f);
			}
			else
			{
				GUI.backgroundColor = Color.Lerp(new Color(0.1f, 0.7f, 0.2f), new Color(0.05f, 0.4f, 0.1f), num9);
			}
			Vector2Int val = snakeBody[num8];
			float num10 = num2 + (float)((Vector2Int)(ref val)).x * num;
			val = snakeBody[num8];
			float num11 = num3 + (float)((Vector2Int)(ref val)).y * num;
			GUI.Button(new Rect(num10 + 0.5f, num11 + 0.5f, num - 1f, num - 1f), "");
		}
		GUI.backgroundColor = guiColorB;
		string text2 = (snakeUseAI ? "AI is playing!" : "Arrow Keys / WASD to move");
		GUI.Label(new Rect(num2, num3 + 15f * num + 8f, num * 20f, 20f), "<size=11>" + text2 + "</size>");
		float num12 = num3 + 15f * num + 30f;
		GUI.backgroundColor = new Color(0.14f, 0.14f, 0.18f, 0.9f);
		GUI.Box(new Rect(num2 - 5f, num12, num * 20f + 10f, 90f), "");
		GUI.backgroundColor = Color.clear;
		GUI.color = guiColorA;
		GUI.Label(new Rect(num2, num12 + 4f, num * 20f, 18f), "<size=13><b>How to Play</b></size>");
		GUI.color = Color.white;
		GUI.Label(new Rect(num2 + 5f, num12 + 22f, num * 20f, 16f), "<size=11>Use Arrow Keys or WASD to guide the snake</size>");
		GUI.Label(new Rect(num2 + 5f, num12 + 38f, num * 20f, 16f), "<size=11>Eat the red food to grow and earn +10 points</size>");
		GUI.Label(new Rect(num2 + 5f, num12 + 54f, num * 20f, 16f), "<size=11>Don't hit the walls or your own tail!</size>");
		GUI.Label(new Rect(num2 + 5f, num12 + 70f, num * 20f, 16f), "<size=11>Toggle AI to let the computer play for you</size>");
		if (!snakeAlive && snakeGameActive)
		{
			float num13 = num3 + 15f * num + 32f;
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.7f);
			GUI.Box(new Rect(num2 - 5f, num13 - 5f, num * 20f + 10f, 100f), "");
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.white;
			GUI.Label(new Rect(num2, num13, num * 20f, 25f), "<size=14><b>Game Over!</b></size>");
			GUI.Label(new Rect(num2, num13 + 22f, num * 20f, 20f), $"<size=13>Score: {snakeScore}  Length: {snakeBody.Count}</size>");
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(num2 + num * 4f, num13 + 55f, 100f, 28f), "Play Again"))
			{
				SnakeNewGame();
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
			if (GUI.Button(new Rect(num2 + num * 4f + 110f, num13 + 55f, 100f, 28f), "Quit"))
			{
				snakeGameActive = false;
				snakeScore = 0;
			}
			GUI.color = Color.white;
		}
		GUI.EndScrollView();
	}

	private void SnakeNewGame()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		snakeGrid = new int[20, 15];
		snakeBody = new List<Vector2Int>();
		int num = 10;
		int num2 = 7;
		snakeBody.Add(new Vector2Int(num, num2));
		snakeBody.Add(new Vector2Int(num - 1, num2));
		snakeBody.Add(new Vector2Int(num - 2, num2));
		snakeDir = new Vector2Int(1, 0);
		snakeScore = 0;
		snakeAlive = true;
		snakeGameActive = true;
		snakeMoveTimer = 0f;
		snakeMoveInterval = 0.25f;
		snakePath = null;
		SnakeSpawnFood();
	}

	private void SnakeSpawnFood()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				if (snakeGrid[j, i] == 0)
				{
					list.Add(new Vector2Int(j, i));
				}
			}
		}
		if (list.Count > 0)
		{
			snakeFood = list[Random.Range(0, list.Count)];
		}
	}

	private void SnakeStep()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		Vector2Int val = snakeBody[0];
		Vector2Int val2 = val + snakeDir;
		if (((Vector2Int)(ref val2)).x < 0 || ((Vector2Int)(ref val2)).x >= 20 || ((Vector2Int)(ref val2)).y < 0 || ((Vector2Int)(ref val2)).y >= 15)
		{
			snakeAlive = false;
			if (snakeScore > snakeBestScore)
			{
				snakeBestScore = snakeScore;
			}
			return;
		}
		for (int i = 0; i < snakeBody.Count; i++)
		{
			if (snakeBody[i] == val2)
			{
				snakeAlive = false;
				if (snakeScore > snakeBestScore)
				{
					snakeBestScore = snakeScore;
				}
				return;
			}
		}
		snakeBody.Insert(0, val2);
		if (val2 == snakeFood)
		{
			snakeScore += 10;
			if (snakeMoveInterval > 0.08f)
			{
				snakeMoveInterval = Mathf.Max(0.08f, snakeMoveInterval - 0.001f);
			}
			SnakeSpawnFood();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		else
		{
			snakeBody.RemoveAt(snakeBody.Count - 1);
		}
	}

	private void SnakeAIStep()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		Vector2Int val = snakeBody[0];
		Vector2Int val2 = snakeFood - val;
		Vector2Int[] array = (Vector2Int[])(object)new Vector2Int[4]
		{
			new Vector2Int(0, -1),
			new Vector2Int(0, 1),
			new Vector2Int(-1, 0),
			new Vector2Int(1, 0)
		};
		Vector2Int val3 = snakeDir;
		float num = -9999f;
		Vector2Int[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Vector2Int val4 = array2[i];
			if (val4 == -snakeDir)
			{
				continue;
			}
			Vector2Int val5 = val + val4;
			if (((Vector2Int)(ref val5)).x < 0 || ((Vector2Int)(ref val5)).x >= 20 || ((Vector2Int)(ref val5)).y < 0 || ((Vector2Int)(ref val5)).y >= 15)
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < snakeBody.Count - 1; j++)
			{
				if (snakeBody[j] == val5)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			float num2 = 0f;
			if (((Vector2Int)(ref val4)).x == ((Vector2Int)(ref val2)).x && ((Vector2Int)(ref val2)).x != 0)
			{
				num2 += 2f;
			}
			if (((Vector2Int)(ref val4)).y == ((Vector2Int)(ref val2)).y && ((Vector2Int)(ref val2)).y != 0)
			{
				num2 += 2f;
			}
			int num3 = 0;
			Vector2Int[] array3 = array;
			foreach (Vector2Int val6 in array3)
			{
				Vector2Int val7 = val5 + val6;
				if (((Vector2Int)(ref val7)).x < 0 || ((Vector2Int)(ref val7)).x >= 20 || ((Vector2Int)(ref val7)).y < 0 || ((Vector2Int)(ref val7)).y >= 15)
				{
					continue;
				}
				bool flag2 = false;
				for (int l = 0; l < snakeBody.Count - 1; l++)
				{
					if (snakeBody[l] == val7)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					num3++;
				}
			}
			num2 += (float)num3 * 0.5f;
			if (num3 == 0)
			{
				num2 -= 10f;
			}
			if (num2 > num)
			{
				num = num2;
				val3 = val4;
			}
		}
		if (val3 != -snakeDir || snakeBody.Count <= 3)
		{
			snakeDir = val3;
		}
	}

	private void DrawConnectFour()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		if (c4Grid == null)
		{
			C4NewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Connect Four</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"You: 1  |  AI: 2  |  Wins: {c4ScoreX}  Losses: {c4ScoreO}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(450f, 46f, 80f, 22f), "New Game"))
		{
			C4NewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		string[] array = new string[3] { "Easy", "Normal", "Hard" };
		GUI.Label(new Rect(170f, 68f, 60f, 20f), "Difficulty:");
		for (int i = 0; i < 3; i++)
		{
			GUI.backgroundColor = ((c4Diff == i) ? guiColorA : guiColorB);
			if (GUI.Button(new Rect(240f + (float)i * 65f, 68f, 58f, 20f), array[i]))
			{
				c4Diff = i;
				C4NewGame();
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		c4ScrollPos = GUI.BeginScrollView(new Rect(170f, 90f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 78f), c4ScrollPos, new Rect(0f, 0f, 500f, 480f), false, true);
		float num = 50f;
		float num2 = 6f;
		float num3 = 60f;
		float num4 = 10f;
		GUI.backgroundColor = new Color(0.1f, 0.1f, 0.25f);
		GUI.Box(new Rect(num3 - 5f, num4 - 5f, 7f * (num + num2) + 10f, 6f * (num + num2) + 10f), "");
		for (int j = 0; j < 6; j++)
		{
			for (int k = 0; k < 7; k++)
			{
				float num5 = num3 + (float)k * (num + num2);
				float num6 = num4 + (float)j * (num + num2);
				int num7 = c4Grid[k, j];
				GUI.backgroundColor = new Color(0.06f, 0.06f, 0.1f);
				GUI.Button(new Rect(num5 + 0.5f, num6 + 0.5f, num - 1f, num - 1f), "");
				switch (num7)
				{
				case 1:
					GUI.backgroundColor = new Color(0.3f, 0.7f, 1f);
					GUI.Button(new Rect(num5 + 5f, num6 + 5f, num - 10f, num - 10f), "");
					GUI.backgroundColor = new Color(0.1f, 0.35f, 0.7f);
					GUI.Button(new Rect(num5 + 14f, num6 + 14f, num - 28f, num - 28f), "");
					break;
				case 2:
					GUI.backgroundColor = new Color(1f, 0.4f, 0.35f);
					GUI.Button(new Rect(num5 + 5f, num6 + 5f, num - 10f, num - 10f), "");
					GUI.backgroundColor = new Color(0.7f, 0.15f, 0.1f);
					GUI.Button(new Rect(num5 + 14f, num6 + 14f, num - 28f, num - 28f), "");
					break;
				}
			}
		}
		if (c4Winner == 0 && c4Turn == 0 && Time.time > c4AICooldown)
		{
			for (int l = 0; l < 7; l++)
			{
				float num8 = num3 + (float)l * (num + num2);
				float num9 = num4 - num - 5f;
				GUI.backgroundColor = guiColorA;
				if (!GUI.Button(new Rect(num8 + 0.5f, num9 + 0.5f, num - 1f, num - 1f), "^") || !C4Drop(l, 1))
				{
					continue;
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				switch (C4CheckWin())
				{
				case 1:
					c4Winner = 1;
					c4ScoreX++;
					continue;
				case 2:
					c4Winner = 2;
					c4ScoreO++;
					continue;
				}
				if (C4IsFull())
				{
					c4Winner = 3;
					continue;
				}
				c4Turn = 1;
				c4AICooldown = Time.time + 0.5f;
				((MonoBehaviour)this).Invoke("C4AIMove", 0.45f);
			}
		}
		GUI.backgroundColor = guiColorB;
		if (c4Winner != 0)
		{
			float num10 = num4 + 6f * (num + num2) + 15f;
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.7f);
			GUI.Box(new Rect(num3 - 5f, num10 - 5f, 7f * (num + num2) + 10f, 90f), "");
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.white;
			string text = ((c4Winner == 1) ? "You Win!" : ((c4Winner == 2) ? "AI Wins!" : "Draw!"));
			GUI.Label(new Rect(num3, num10, 7f * (num + num2), 25f), "<size=16><b>" + text + "</b></size>");
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(num3 + num * 1.5f, num10 + 35f, 100f, 28f), "Play Again"))
			{
				C4NewGame();
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
			if (GUI.Button(new Rect(num3 + num * 1.5f + 110f, num10 + 35f, 100f, 28f), "Quit"))
			{
				c4Winner = 4;
				c4Grid = null;
			}
			GUI.color = Color.white;
		}
		float num11 = num4 + 6f * (num + num2) + 15f;
		if (c4Winner != 0)
		{
			num11 += 100f;
		}
		GUI.backgroundColor = new Color(0.14f, 0.14f, 0.18f, 0.9f);
		GUI.Box(new Rect(num3 - 5f, num11, 7f * (num + num2) + 10f, 90f), "");
		GUI.backgroundColor = Color.clear;
		GUI.color = guiColorA;
		GUI.Label(new Rect(num3, num11 + 4f, 7f * (num + num2), 18f), "<size=13><b>How to Play</b></size>");
		GUI.color = Color.white;
		GUI.Label(new Rect(num3 + 5f, num11 + 22f, 7f * (num + num2), 16f), "<size=11>Click the ^ buttons above a column to drop your piece</size>");
		GUI.Label(new Rect(num3 + 5f, num11 + 38f, 7f * (num + num2), 16f), "<size=11>First to connect 4 pieces in a row wins</size>");
		GUI.Label(new Rect(num3 + 5f, num11 + 54f, 7f * (num + num2), 16f), "<size=11>Rows, columns, and diagonals all count</size>");
		GUI.Label(new Rect(num3 + 5f, num11 + 70f, 7f * (num + num2), 16f), "<size=11>Blue = You  |  Red = AI  |  Adjust difficulty above</size>");
		GUI.EndScrollView();
	}

	private void C4NewGame()
	{
		c4Grid = new int[7, 6];
		c4Winner = 0;
		c4Turn = 0;
		c4AICooldown = 0f;
	}

	private bool C4Drop(int col, int player)
	{
		for (int num = 5; num >= 0; num--)
		{
			if (c4Grid[col, num] == 0)
			{
				c4Grid[col, num] = player;
				return true;
			}
		}
		return false;
	}

	private bool C4IsFull()
	{
		for (int i = 0; i < 7; i++)
		{
			if (c4Grid[i, 0] == 0)
			{
				return false;
			}
		}
		return true;
	}

	private int C4CheckWin()
	{
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				int num = c4Grid[i, j];
				if (num == 0)
				{
					continue;
				}
				int[][] array = new int[4][]
				{
					new int[2] { 1, 0 },
					new int[2] { 0, 1 },
					new int[2] { 1, 1 },
					new int[2] { 1, -1 }
				};
				int[][] array2 = array;
				foreach (int[] array3 in array2)
				{
					bool flag = true;
					for (int l = 1; l < 4; l++)
					{
						int num2 = i + array3[0] * l;
						int num3 = j + array3[1] * l;
						if (num2 < 0 || num2 >= 7 || num3 < 0 || num3 >= 6 || c4Grid[num2, num3] != num)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return num;
					}
				}
			}
		}
		return 0;
	}

	private int C4Eval()
	{
		switch (C4CheckWin())
		{
		case 2:
			return 1000;
		case 1:
			return -1000;
		default:
		{
			int num = 0;
			for (int i = 0; i < 7; i++)
			{
				if (c4Grid[i, 0] == 0)
				{
					int num2 = num;
					int num3;
					switch (i)
					{
					default:
						num3 = 1;
						break;
					case 2:
					case 4:
						num3 = 2;
						break;
					case 3:
						num3 = 3;
						break;
					}
					num = num2 + num3;
				}
			}
			return num;
		}
		}
	}

	private void C4AIMove()
	{
		if (c4Winner != 0 || c4Turn != 1)
		{
			return;
		}
		int num = 3;
		if (c4Diff == 0)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 7; i++)
			{
				if (c4Grid[i, 0] == 0)
				{
					list.Add(i);
				}
			}
			num = list[Random.Range(0, list.Count)];
		}
		else if (c4Diff == 1)
		{
			num = C4MinimaxRoot(3);
			if (Random.value < 0.3f)
			{
				List<int> list2 = new List<int>();
				for (int j = 0; j < 7; j++)
				{
					if (c4Grid[j, 0] == 0)
					{
						list2.Add(j);
					}
				}
				num = list2[Random.Range(0, list2.Count)];
			}
		}
		else
		{
			num = C4MinimaxRoot(5);
		}
		C4Drop(num, 2);
		switch (C4CheckWin())
		{
		case 2:
			c4Winner = 2;
			c4ScoreO++;
			return;
		case 1:
			c4Winner = 1;
			c4ScoreX++;
			return;
		}
		if (C4IsFull())
		{
			c4Winner = 3;
		}
		else
		{
			c4Turn = 0;
		}
	}

	private int C4MinimaxRoot(int depth)
	{
		int num = -99999;
		int result = 3;
		List<int> list = new List<int> { 3, 2, 4, 1, 5, 0, 6 };
		foreach (int item in list)
		{
			if (c4Grid[item, 0] != 0)
			{
				continue;
			}
			C4Drop(item, 2);
			int num2 = C4MinimaxHelper(depth - 1, maximizing: false, -99999, 99999);
			for (int i = 0; i < 6; i++)
			{
				if (c4Grid[item, i] != 0)
				{
					c4Grid[item, i] = 0;
					break;
				}
			}
			if (num2 > num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}

	private int C4MinimaxHelper(int depth, bool maximizing, int alpha, int beta)
	{
		switch (C4CheckWin())
		{
		case 2:
			return 1000 + depth;
		case 1:
			return -1000 - depth;
		default:
		{
			if (depth == 0 || C4IsFull())
			{
				return C4Eval();
			}
			if (maximizing)
			{
				int num = -99999;
				for (int i = 0; i < 7; i++)
				{
					if (c4Grid[i, 0] != 0)
					{
						continue;
					}
					C4Drop(i, 2);
					int val = C4MinimaxHelper(depth - 1, maximizing: false, alpha, beta);
					int num2 = -1;
					for (int j = 0; j < 6; j++)
					{
						if (c4Grid[i, j] != 0)
						{
							num2 = j;
							break;
						}
					}
					if (num2 >= 0)
					{
						c4Grid[i, num2] = 0;
					}
					num = Math.Max(num, val);
					alpha = Math.Max(alpha, val);
					if (beta <= alpha)
					{
						break;
					}
				}
				return num;
			}
			int num3 = 99999;
			for (int k = 0; k < 7; k++)
			{
				if (c4Grid[k, 0] != 0)
				{
					continue;
				}
				C4Drop(k, 1);
				int val2 = C4MinimaxHelper(depth - 1, maximizing: true, alpha, beta);
				int num4 = -1;
				for (int l = 0; l < 6; l++)
				{
					if (c4Grid[k, l] != 0)
					{
						num4 = l;
						break;
					}
				}
				if (num4 >= 0)
				{
					c4Grid[k, num4] = 0;
				}
				num3 = Math.Min(num3, val2);
				beta = Math.Min(beta, val2);
				if (beta <= alpha)
				{
					break;
				}
			}
			return num3;
		}
		}
	}

	private int C4FirstEmpty(int col)
	{
		for (int num = 5; num >= 0; num--)
		{
			if (c4Grid[col, num] == 0)
			{
				return num;
			}
		}
		return -1;
	}

	private void DrawFlappyBird()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Invalid comparison between Unknown and I4
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Expected O, but got Unknown
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Invalid comparison between Unknown and I4
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Invalid comparison between Unknown and I4
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Invalid comparison between Unknown and I4
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Expected O, but got Unknown
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Expected O, but got Unknown
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0952: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		if (fbPipeX == null)
		{
			FBNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Flappy Bird</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {fbScore}  Best: {fbBestScore}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(450f, 46f, 80f, 22f), "New Game"))
		{
			FBNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		fbScrollPos = GUI.BeginScrollView(new Rect(170f, 68f, ((Rect)(ref guiRect)).width - 170f, ((Rect)(ref guiRect)).height - 56f), fbScrollPos, new Rect(0f, 0f, 500f, 500f), false, true);
		float num = 20f;
		float num2 = 10f;
		float num3 = 460f;
		float num4 = 350f;
		float num5 = 20f;
		float num6 = 40f;
		float num7 = 90f;
		float num8 = 30f;
		if (fbAlive && fbGameActive)
		{
			if ((int)Event.current.type == 0 || ((int)Event.current.type == 4 && ((int)Event.current.keyCode == 32 || (int)Event.current.keyCode == 273 || (int)Event.current.keyCode == 119)))
			{
				fbBirdVel = -260f;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				Event.current.Use();
			}
			float deltaTime = Time.deltaTime;
			fbBirdVel += 420f * deltaTime;
			fbBirdY += fbBirdVel * deltaTime;
			fbGroundOffset = (fbGroundOffset + 100f * deltaTime) % 20f;
			for (int num9 = fbPipeX.Count - 1; num9 >= 0; num9--)
			{
				fbPipeX[num9] -= 120f * deltaTime;
				if (fbPipeX[num9] + num6 < fbBirdX)
				{
					fbPipeX.RemoveAt(num9);
					fbPipeGap.RemoveAt(num9);
				}
			}
			if (fbPipeX.Count == 0 || fbPipeX[fbPipeX.Count - 1] < num + num3 - 160f)
			{
				float num10 = num2 + 40f;
				float num11 = num2 + num4 - num8 - num7 - 40f;
				float item = Random.Range(num10, num11);
				fbPipeX.Add(num + num3);
				fbPipeGap.Add(item);
			}
			for (int i = 0; i < fbPipeX.Count; i++)
			{
				float num12 = fbPipeX[i];
				float num13 = fbPipeGap[i];
				if (num12 + num6 > fbBirdX && num12 < fbBirdX + num5 && (fbBirdY < num13 || fbBirdY + num5 > num13 + num7))
				{
					fbAlive = false;
					if (fbScore > fbBestScore)
					{
						fbBestScore = fbScore;
					}
				}
				if (num12 + num6 > fbBirdX + num5 && num12 + num6 < fbBirdX + num5 + 120f * deltaTime)
				{
					fbScore++;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
			if (fbBirdY < num2 || fbBirdY + num5 > num2 + num4 - num8)
			{
				fbAlive = false;
				if (fbScore > fbBestScore)
				{
					fbBestScore = fbScore;
				}
			}
		}
		GUI.backgroundColor = new Color(0.53f, 0.81f, 0.92f);
		GUI.Box(new Rect(num, num2, num3, num4 - num8), "");
		GUI.backgroundColor = guiColorB;
		for (int j = 0; j < fbPipeX.Count; j++)
		{
			float num14 = fbPipeX[j];
			float num15 = fbPipeGap[j];
			if (!(num14 > num + num3) && !(num14 + num6 < num))
			{
				GUI.backgroundColor = new Color(0.2f, 0.7f, 0.3f);
				GUI.Button(new Rect(num14, num2, num6, num15 - num2), "");
				GUI.Button(new Rect(num14, num15 + num7, num6, num2 + num4 - num8 - num15 - num7), "");
				GUI.backgroundColor = new Color(0.15f, 0.55f, 0.25f);
				GUI.Button(new Rect(num14 + 3f, num2, 6f, num15 - num2), "");
				GUI.Button(new Rect(num14 + 3f, num15 + num7, 6f, num2 + num4 - num8 - num15 - num7), "");
			}
		}
		GUI.backgroundColor = new Color(1f, 0.85f, 0.2f);
		float num16 = Mathf.Clamp(fbBirdY, num2, num2 + num4 - num8 - num5);
		GUI.Button(new Rect(fbBirdX, num16, num5, num5), "");
		GUI.backgroundColor = new Color(1f, 0.6f, 0.1f);
		GUI.Button(new Rect(fbBirdX + num5 - 4f, num16 + 5f, 8f, 6f), "");
		GUI.backgroundColor = Color.clear;
		GUI.color = Color.white;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 20,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)1
		};
		GUI.Label(new Rect(num, num2 + 5f, num3, 30f), fbScore.ToString(), val);
		GUI.color = Color.white;
		GUI.backgroundColor = new Color(0.85f, 0.75f, 0.4f);
		GUI.Box(new Rect(num, num2 + num4 - num8, num3, num8), "");
		for (float num17 = 0f - fbGroundOffset; num17 < num3; num17 += 20f)
		{
			GUI.backgroundColor = new Color(0.7f, 0.6f, 0.3f);
			GUI.Box(new Rect(num + num17, num2 + num4 - num8, 10f, num8), "");
		}
		if (!fbAlive && fbGameActive)
		{
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.6f);
			GUI.Box(new Rect(num, num2, num3, num4), "");
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.white;
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + num4 * 0.3f, num3, 30f), "Game Over!", val2);
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 13,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + num4 * 0.3f + 28f, num3, 20f), $"Score: {fbScore}  Best: {fbBestScore}", val3);
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(num + num3 / 2f - 60f, num2 + num4 * 0.3f + 60f, 120f, 30f), "Play Again"))
			{
				FBNewGame();
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
			if (GUI.Button(new Rect(num + num3 / 2f - 60f, num2 + num4 * 0.3f + 95f, 120f, 30f), "Quit"))
			{
				fbGameActive = false;
				fbScore = 0;
			}
			GUI.color = Color.white;
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num, num2 + num4 + 5f, num3, 16f), "<size=11>Click, Space, or W/Up to flap</size>");
		float num18 = num2 + num4 + 25f;
		GUI.backgroundColor = new Color(0.14f, 0.14f, 0.18f, 0.9f);
		GUI.Box(new Rect(num - 5f, num18, num3 + 10f, 80f), "");
		GUI.backgroundColor = Color.clear;
		GUI.color = guiColorA;
		GUI.Label(new Rect(num, num18 + 4f, num3, 18f), "<size=13><b>How to Play</b></size>");
		GUI.color = Color.white;
		GUI.Label(new Rect(num + 5f, num18 + 22f, num3, 16f), "<size=11>Click, Space, or W/Up arrow to flap and gain altitude</size>");
		GUI.Label(new Rect(num + 5f, num18 + 38f, num3, 16f), "<size=11>Navigate through the gaps between green pipes</size>");
		GUI.Label(new Rect(num + 5f, num18 + 54f, num3, 16f), "<size=11>Score increases for each pipe you pass. Don't hit the ground!</size>");
		GUI.EndScrollView();
	}

	private void FBNewGame()
	{
		float num = 350f;
		float num2 = 30f;
		fbBirdY = (num - num2) / 2f - 10f;
		fbBirdVel = 0f;
		fbBirdX = 80f;
		fbPipeX = new List<float>();
		fbPipeGap = new List<float>();
		fbScore = 0;
		fbAlive = true;
		fbGameActive = true;
		fbGroundOffset = 0f;
	}

	private void DrawMinesweeper()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Expected O, but got Unknown
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		if (msGrid == null)
		{
			MSNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Minesweeper</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Mines: {msMines}  Flags Left: {msFlagsLeft}  Time: {msTimer:F1}s");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 90f, 22f), msFlagMode ? "⛏ Dig" : "\ud83d\udea9 Flag"))
		{
			msFlagMode = !msFlagMode;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(530f, 46f, 80f, 22f), "New Game"))
		{
			MSNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (!msGameOver && msStarted)
		{
			msTimer += Time.deltaTime;
		}
		float num = 26f;
		float num2 = 180f;
		float num3 = 72f;
		for (int i = 0; i < msRows; i++)
		{
			for (int j = 0; j < msCols; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				if (msRevealed[i, j])
				{
					if (msGrid[i, j] == -1)
					{
						GUI.backgroundColor = new Color(0.9f, 0.2f, 0.2f);
						GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "\ud83d\udca3");
						continue;
					}
					GUI.backgroundColor = new Color(0.75f, 0.75f, 0.8f);
					int num6 = msGrid[i, j];
					string text = ((num6 > 0) ? num6.ToString() : "");
					GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), text);
					continue;
				}
				if (msFlagged[i, j])
				{
					GUI.backgroundColor = new Color(1f, 0.85f, 0.4f);
					if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "\ud83d\udea9") && !msGameOver)
					{
						msFlagged[i, j] = false;
						msFlagsLeft++;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					continue;
				}
				GUI.backgroundColor = new Color(0.55f, 0.6f, 0.65f);
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "") && !msGameOver)
				{
					if (msFlagMode)
					{
						msFlagged[i, j] = true;
						msFlagsLeft--;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					else
					{
						MSReveal(i, j);
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
				}
			}
		}
		if (msGameOver)
		{
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.white;
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			string text2 = (msWon ? "You Win!" : "Game Over!");
			GUI.Label(new Rect(num2, num3 + (float)msRows * num + 5f, (float)msCols * num, 25f), text2, val);
			GUI.color = Color.white;
		}
		GUI.backgroundColor = guiColorB;
	}

	private void MSNewGame()
	{
		msGrid = new int[msRows, msCols];
		msRevealed = new bool[msRows, msCols];
		msFlagged = new bool[msRows, msCols];
		int num = 0;
		while (num < msMines)
		{
			int num2 = Random.Range(0, msRows);
			int num3 = Random.Range(0, msCols);
			if (msGrid[num2, num3] != -1)
			{
				msGrid[num2, num3] = -1;
				num++;
			}
		}
		for (int i = 0; i < msRows; i++)
		{
			for (int j = 0; j < msCols; j++)
			{
				if (msGrid[i, j] == -1)
				{
					continue;
				}
				int num4 = 0;
				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						int num5 = i + k;
						int num6 = j + l;
						if (num5 >= 0 && num5 < msRows && num6 >= 0 && num6 < msCols && msGrid[num5, num6] == -1)
						{
							num4++;
						}
					}
				}
				msGrid[i, j] = num4;
			}
		}
		msFlagsLeft = msMines;
		msGameOver = false;
		msWon = false;
		msStarted = false;
		msTimer = 0f;
		msFlagMode = false;
	}

	private void MSReveal(int r, int c)
	{
		if (r < 0 || r >= msRows || c < 0 || c >= msCols || msRevealed[r, c] || msFlagged[r, c])
		{
			return;
		}
		msRevealed[r, c] = true;
		msStarted = true;
		if (msGrid[r, c] == -1)
		{
			msGameOver = true;
			for (int i = 0; i < msRows; i++)
			{
				for (int j = 0; j < msCols; j++)
				{
					if (msGrid[i, j] == -1)
					{
						msRevealed[i, j] = true;
					}
				}
			}
			return;
		}
		if (msGrid[r, c] == 0)
		{
			for (int k = -1; k <= 1; k++)
			{
				for (int l = -1; l <= 1; l++)
				{
					MSReveal(r + k, c + l);
				}
			}
		}
		int num = 0;
		for (int m = 0; m < msRows; m++)
		{
			for (int n = 0; n < msCols; n++)
			{
				if (!msRevealed[m, n] && msGrid[m, n] != -1)
				{
					num++;
				}
			}
		}
		if (num == 0)
		{
			msGameOver = true;
			msWon = true;
		}
	}

	private void Draw2048()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Invalid comparison between Unknown and I4
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Invalid comparison between Unknown and I4
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Invalid comparison between Unknown and I4
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Invalid comparison between Unknown and I4
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Expected O, but got Unknown
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Invalid comparison between Unknown and I4
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Invalid comparison between Unknown and I4
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Invalid comparison between Unknown and I4
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Invalid comparison between Unknown and I4
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Invalid comparison between Unknown and I4
		if (g4Grid == null)
		{
			G4NewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>2048</size>");
		GUI.Label(new Rect(170f, 48f, 200f, 20f), $"Score: {g4Score}  Best: {g4Best}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			G4NewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		Event current = Event.current;
		if ((int)current.type == 4 && !current.shift && !current.control && !current.alt)
		{
			bool flag = false;
			if ((int)current.keyCode == 273 || (int)current.keyCode == 119)
			{
				flag = G4Slide(0);
				current.Use();
			}
			else if ((int)current.keyCode == 274 || (int)current.keyCode == 115)
			{
				flag = G4Slide(1);
				current.Use();
			}
			else if ((int)current.keyCode == 276 || (int)current.keyCode == 97)
			{
				flag = G4Slide(2);
				current.Use();
			}
			else if ((int)current.keyCode == 275 || (int)current.keyCode == 100)
			{
				flag = G4Slide(3);
				current.Use();
			}
			if (flag)
			{
				G4Spawn();
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		float num = 60f;
		float num2 = 190f;
		float num3 = 72f;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int num4 = g4Grid[i, j];
				float num5 = num2 + (float)j * num;
				float num6 = num3 + (float)i * num;
				GUI.backgroundColor = G4TileColor(num4);
				GUI.Button(new Rect(num5, num6, num - 2f, num - 2f), (num4 > 0) ? num4.ToString() : "");
			}
		}
		GUI.backgroundColor = guiColorB;
		if (g4Won)
		{
			GUI.color = Color.yellow;
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num2, num3 + 4f * num + 10f, 4f * num, 30f), "You Win! Reached 2048!", val);
			GUI.color = Color.white;
		}
		else if (!G4CanMove())
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num2, num3 + 4f * num + 10f, 4f * num, 30f), "No moves left!", val2);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num2, num3 + 4f * num + 40f, 4f * num, 20f), "<size=11>Arrow keys or WASD to slide tiles</size>");
	}

	private void G4NewGame()
	{
		g4Grid = new int[4, 4];
		g4Score = 0;
		g4Active = true;
		g4Won = false;
		G4Spawn();
		G4Spawn();
	}

	private void G4Spawn()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (g4Grid[i, j] == 0)
				{
					list.Add(i * 4 + j);
				}
			}
		}
		if (list.Count != 0)
		{
			int num = list[Random.Range(0, list.Count)];
			g4Grid[num / 4, num % 4] = ((Random.Range(0, 10) < 9) ? 2 : 4);
		}
	}

	private bool G4Slide(int dir)
	{
		bool result = false;
		int[] array = new int[4] { -1, 1, 0, 0 };
		int[] array2 = new int[4] { 0, 0, -1, 1 };
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					int num = j + array[dir];
					int num2 = k + array2[dir];
					if (num < 0 || num >= 4 || num2 < 0 || num2 >= 4)
					{
						continue;
					}
					if (g4Grid[num, num2] == 0 && g4Grid[j, k] != 0)
					{
						g4Grid[num, num2] = g4Grid[j, k];
						g4Grid[j, k] = 0;
						result = true;
					}
					else if (g4Grid[num, num2] == g4Grid[j, k] && g4Grid[j, k] != 0)
					{
						g4Grid[num, num2] *= 2;
						g4Score += g4Grid[num, num2];
						if (g4Grid[num, num2] == 2048)
						{
							g4Won = true;
						}
						if (g4Score > g4Best)
						{
							g4Best = g4Score;
						}
						g4Grid[j, k] = 0;
						result = true;
					}
				}
			}
		}
		return result;
	}

	private bool G4CanMove()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (g4Grid[i, j] == 0)
				{
					return true;
				}
				if (j < 3 && g4Grid[i, j] == g4Grid[i, j + 1])
				{
					return true;
				}
				if (i < 3 && g4Grid[i, j] == g4Grid[i + 1, j])
				{
					return true;
				}
			}
		}
		return false;
	}

	private Color G4TileColor(int v)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(v switch
		{
			0 => new Color(0.18f, 0.18f, 0.22f), 
			2 => new Color(0.9f, 0.9f, 0.85f), 
			4 => new Color(0.9f, 0.88f, 0.7f), 
			8 => new Color(0.95f, 0.65f, 0.3f), 
			16 => new Color(0.95f, 0.5f, 0.25f), 
			32 => new Color(0.9f, 0.3f, 0.2f), 
			64 => new Color(0.85f, 0.15f, 0.15f), 
			128 => new Color(0.95f, 0.9f, 0.4f), 
			256 => new Color(0.95f, 0.88f, 0.35f), 
			512 => new Color(0.95f, 0.85f, 0.3f), 
			1024 => new Color(0.95f, 0.8f, 0.25f), 
			_ => new Color(0.95f, 0.75f, 0.2f), 
		});
	}

	private void DrawPong()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Invalid comparison between Unknown and I4
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Invalid comparison between Unknown and I4
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Invalid comparison between Unknown and I4
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Invalid comparison between Unknown and I4
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Invalid comparison between Unknown and I4
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Invalid comparison between Unknown and I4
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Expected O, but got Unknown
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Pong</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"You: {pongPScore}  AI: {pongEScore}  First to 10");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			PongReset();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (!pongStarted)
		{
			PongReset();
		}
		float num = 190f;
		float num2 = 72f;
		float num3 = 10f;
		float num4 = 50f;
		Event current = Event.current;
		if ((int)current.type == 0)
		{
			pongPlayerY = Mathf.Clamp(current.mousePosition.y - num2 - num4 / 2f, 0f, pongFieldH - num4);
		}
		if ((int)current.type == 4)
		{
			if ((int)current.keyCode == 273 || (int)current.keyCode == 119)
			{
				pongPlayerY = Mathf.Max(0f, pongPlayerY - 15f);
			}
			if ((int)current.keyCode == 274 || (int)current.keyCode == 115)
			{
				pongPlayerY = Mathf.Min(pongFieldH - num4, pongPlayerY + 15f);
			}
		}
		float num5 = 3.5f;
		float num6 = pongEnemyY + num4 / 2f - (pongBallY + 4f);
		if (num6 > 5f)
		{
			pongEnemyY -= num5;
		}
		else if (num6 < -5f)
		{
			pongEnemyY += num5;
		}
		pongEnemyY = Mathf.Clamp(pongEnemyY, 0f, pongFieldH - num4);
		pongBallX += pongBallVX * Time.deltaTime;
		pongBallY += pongBallVY * Time.deltaTime;
		if (pongBallY <= 0f || pongBallY + 8f >= pongFieldH)
		{
			pongBallVY = 0f - pongBallVY;
			pongBallY = Mathf.Clamp(pongBallY, 0f, pongFieldH - 8f);
		}
		float num7 = num + 2f;
		float num8 = num + pongFieldW - num3 - 2f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(num7, num2 + pongPlayerY, num3, num4);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(num8, num2 + pongEnemyY, num3, num4);
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(num + pongBallX, num2 + pongBallY, 8f, 8f);
		if (((Rect)(ref val3)).Overlaps(val) && pongBallVX < 0f)
		{
			pongBallVX = (0f - pongBallVX) * 1.05f;
			float num9 = (pongBallY + 4f - (pongPlayerY + num4 / 2f)) / (num4 / 2f);
			pongBallVY += num9 * 200f;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (((Rect)(ref val3)).Overlaps(val2) && pongBallVX > 0f)
		{
			pongBallVX = (0f - pongBallVX) * 1.05f;
			float num10 = (pongBallY + 4f - (pongEnemyY + num4 / 2f)) / (num4 / 2f);
			pongBallVY += num10 * 200f;
		}
		if (pongBallX < -10f)
		{
			pongEScore++;
			PongServe(1);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (pongBallX > pongFieldW + 10f)
		{
			pongPScore++;
			PongServe(-1);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (pongPScore >= 10 || pongEScore >= 10)
		{
			pongStarted = false;
			GUI.color = Color.yellow;
			GUIStyle val4 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			string text = ((pongPScore >= 10) ? "You Win!" : "AI Wins!");
			GUI.Label(new Rect(num, num2 + pongFieldH / 2f - 15f, pongFieldW, 30f), text, val4);
			GUI.color = Color.white;
		}
		GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
		GUI.Button(new Rect(num, num2, pongFieldW, pongFieldH), "");
		GUI.backgroundColor = new Color(1f, 1f, 1f);
		GUI.Button(new Rect(num + pongFieldW / 2f - 1f, num2, 2f, pongFieldH), "");
		GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
		GUI.Button(val, "");
		GUI.Button(val2, "");
		GUI.backgroundColor = Color.yellow;
		GUI.Button(val3, "");
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num, num2 + pongFieldH + 5f, pongFieldW, 16f), "<size=11>Mouse or W/S to move paddle</size>");
	}

	private void PongReset()
	{
		pongPlayerY = pongFieldH / 2f - 25f;
		pongEnemyY = pongFieldH / 2f - 25f;
		pongPScore = 0;
		pongEScore = 0;
		pongStarted = true;
		PongServe(1);
	}

	private void PongServe(int dir)
	{
		pongBallX = pongFieldW / 2f - 4f;
		pongBallY = pongFieldH / 2f - 4f;
		pongBallVX = (float)dir * 250f;
		pongBallVY = Random.Range(-100f, 100f);
	}

	private void DrawSimon()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		if (simonPattern == null)
		{
			SimonNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Simon Says</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {simonScore}  High: {simonHigh}");
		GUI.backgroundColor = guiColorB;
		if (simonPhase == 2)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(240f, 100f, 200f, 30f), "Game Over! Watch the pattern.", val);
			GUI.color = Color.white;
		}
		float num = 100f;
		float num2 = 100f;
		float num3 = 200f;
		float num4 = 72f;
		float num5 = 10f;
		for (int i = 0; i < 4; i++)
		{
			float num6 = num3 + (float)(i % 2) * (num + num5);
			float num7 = num4 + (float)(i / 2) * (num2 + num5);
			Color val2 = simonCols[i];
			GUI.backgroundColor = ((simonFlash && simonFlashI == i) ? Color.white : val2);
			if (!GUI.Button(new Rect(num6, num7, num, num2), "") || simonPhase != 1)
			{
				continue;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			if (i == simonPattern[simonPI])
			{
				simonPI++;
				if (simonPI >= simonPattern.Length)
				{
					simonScore++;
					if (simonScore > simonHigh)
					{
						simonHigh = simonScore;
					}
					SimonNextRound();
				}
			}
			else
			{
				simonPhase = 2;
				if (simonScore > simonHigh)
				{
					simonHigh = simonScore;
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num3, num4 + 2f * (num2 + num5) + 5f, 2f * num + num5, 20f), "<size=11>Watch the sequence, then repeat it!</size>");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(num3 + num / 2f, num4 + 2f * (num2 + num5) + 28f, num + num5, 22f), "Restart"))
		{
			SimonNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (simonPhase != 0)
		{
			return;
		}
		simonTimer -= Time.deltaTime;
		if (simonTimer <= 0f)
		{
			if (simonFlashCount < simonPattern.Length)
			{
				simonFlash = true;
				simonFlashI = simonPattern[simonFlashCount];
				simonFlashCount++;
				simonTimer = 0.6f;
			}
			else
			{
				simonFlash = false;
				simonPhase = 1;
				simonPI = 0;
			}
		}
		if (simonFlash && simonTimer < 0.3f)
		{
			simonFlash = false;
		}
	}

	private void SimonNewGame()
	{
		simonPattern = new int[1] { Random.Range(0, 4) };
		simonScore = 0;
		simonPhase = 0;
		simonPI = 0;
		simonTimer = 1f;
		simonFlashCount = 0;
		simonFlash = false;
	}

	private void SimonNextRound()
	{
		int[] array = simonPattern;
		simonPattern = new int[array.Length + 1];
		for (int i = 0; i < array.Length; i++)
		{
			simonPattern[i] = array[i];
		}
		simonPattern[array.Length] = Random.Range(0, 4);
		simonPhase = 0;
		simonPI = 0;
		simonTimer = 1f;
		simonFlashCount = 0;
	}

	private void DrawHangman()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Expected O, but got Unknown
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Expected O, but got Unknown
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(hmWord))
		{
			HMNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Hangman</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Wrong: {hmWrong}/6");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Word"))
		{
			HMNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		string text = "";
		string text2 = hmWord;
		for (int i = 0; i < text2.Length; i++)
		{
			char value = text2[i];
			text += ((hmGuesses.IndexOf(value) >= 0) ? (value + " ") : "_ ");
		}
		GUI.color = Color.white;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 20,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		GUI.Label(new Rect(200f, 72f, 280f, 35f), text.Trim(), val);
		GUI.color = Color.white;
		string[] source = new string[6] { "\ud83d\udc80", "\ud83d\udc40", "\ud83d\udc43", "✋", "\ud83e\uddb6", "\ud83d\ude2b" };
		GUI.Label(new Rect(200f, 108f, 280f, 30f), (hmWrong > 0) ? string.Join(" ", source.Take(hmWrong)) : "");
		float num = 200f;
		float num2 = 145f;
		float num3 = 28f;
		float num4 = 26f;
		string text3 = "abcdefghijklmnopqrstuvwxyz";
		for (int j = 0; j < 26; j++)
		{
			int num5 = j / 13;
			int num6 = j % 13;
			char value2 = text3[j];
			bool flag = hmGuesses.IndexOf(value2) >= 0;
			GUI.backgroundColor = (Color)(flag ? new Color(0.3f, 0.3f, 0.35f) : guiColorA);
			if (flag || hmWon || hmLost || !GUI.Button(new Rect(num + (float)num6 * (num3 + 2f), num2 + (float)num5 * (num4 + 2f), num3, num4), value2.ToString()))
			{
				continue;
			}
			hmGuesses += value2;
			if (hmWord.IndexOf(value2) < 0)
			{
				hmWrong++;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			if (hmWrong >= 6)
			{
				hmLost = true;
			}
			bool flag2 = true;
			string text4 = hmWord;
			foreach (char value3 in text4)
			{
				if (hmGuesses.IndexOf(value3) < 0)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				hmWon = true;
			}
		}
		GUI.backgroundColor = guiColorB;
		if (hmWon)
		{
			GUI.color = Color.green;
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + 70f, 13f * (num3 + 2f), 25f), "You win! The word was: " + hmWord, val2);
			GUI.color = Color.white;
		}
		else if (hmLost)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + 70f, 13f * (num3 + 2f), 25f), "Game Over! Word: " + hmWord, val3);
			GUI.color = Color.white;
		}
	}

	private void HMNewGame()
	{
		hmWord = hmWords[Random.Range(0, hmWords.Length)];
		hmGuesses = "";
		hmWrong = 0;
		hmWon = false;
		hmLost = false;
	}

	private void DrawMemory()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Expected O, but got Unknown
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		if (mmGrid == null)
		{
			MMNewGame();
		}
		if (!mmTexturesLoaded)
		{
			LoadMMTextures();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Memory Match</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Pairs: {mmPairs}/8  Moves: {mmMoves}  Time: {mmTimer:F1}s");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			MMNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (!mmDone)
		{
			mmTimer += Time.deltaTime;
		}
		float num = 70f;
		float num2 = 200f;
		float num3 = 72f;
		if (mmBusy)
		{
			mmFlipBack -= Time.deltaTime;
		}
		if (mmFlipBack <= 0f && mmR2 >= 0)
		{
			if (mmGrid[mmR1, mmC1] != mmGrid[mmR2, mmC2])
			{
				mmOpen[mmR1, mmC1] = false;
				mmOpen[mmR2, mmC2] = false;
			}
			mmR1 = (mmC1 = (mmR2 = (mmC2 = -1)));
			mmBusy = false;
		}
		Rect val = default(Rect);
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				((Rect)(ref val))._002Ector(num4, num5, num - 3f, num - 3f);
				if (mmMatched[i, j] || mmOpen[i, j])
				{
					GUI.backgroundColor = new Color(0.3f, 0.7f, 0.3f);
					GUI.Button(val, "");
					if (mmTextures != null && mmGrid[i, j] < mmTextures.Length && (Object)(object)mmTextures[mmGrid[i, j]] != (Object)null)
					{
						GUI.DrawTexture(new Rect(num4 + 3f, num5 + 3f, num - 9f, num - 9f), (Texture)(object)mmTextures[mmGrid[i, j]], (ScaleMode)0);
					}
					continue;
				}
				GUI.backgroundColor = new Color(0.4f, 0.5f, 0.7f);
				if (!GUI.Button(val, "?") || mmBusy || mmDone)
				{
					continue;
				}
				mmOpen[i, j] = true;
				mmMoves++;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				if (mmR1 < 0)
				{
					mmR1 = i;
					mmC1 = j;
					continue;
				}
				mmR2 = i;
				mmC2 = j;
				if (mmGrid[mmR1, mmC1] == mmGrid[mmR2, mmC2])
				{
					mmMatched[mmR1, mmC1] = true;
					mmMatched[mmR2, mmC2] = true;
					mmPairs++;
					if (mmPairs >= 8)
					{
						mmDone = true;
					}
					mmR1 = (mmC1 = (mmR2 = (mmC2 = -1)));
				}
				else
				{
					mmBusy = true;
					mmFlipBack = 0.6f;
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (mmDone)
		{
			GUI.color = Color.green;
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num2, num3 + 4f * num + 5f, 4f * num, 25f), $"Complete! {mmMoves} moves in {mmTimer:F1}s", val2);
			GUI.color = Color.white;
		}
	}

	private void LoadMMTextures()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		mmTextures = (Texture2D[])(object)new Texture2D[mmImagePaths.Length];
		for (int i = 0; i < mmImagePaths.Length; i++)
		{
			try
			{
				if (File.Exists(mmImagePaths[i]))
				{
					byte[] array = File.ReadAllBytes(mmImagePaths[i]);
					mmTextures[i] = new Texture2D(2, 2);
					ImageConversion.LoadImage(mmTextures[i], array);
				}
			}
			catch
			{
				mmTextures[i] = null;
			}
		}
		mmTexturesLoaded = true;
	}

	private void MMNewGame()
	{
		mmGrid = new int[4, 4];
		mmOpen = new bool[4, 4];
		mmMatched = new bool[4, 4];
		mmR1 = (mmC1 = (mmR2 = (mmC2 = -1)));
		mmBusy = false;
		mmPairs = 0;
		mmMoves = 0;
		mmDone = false;
		mmTimer = 0f;
		int num = mmImagePaths.Length;
		List<int> list = new List<int>();
		for (int i = 0; i < num; i++)
		{
			list.Add(i);
			list.Add(i);
		}
		int num2 = 16 - list.Count;
		for (int j = 0; j < num2; j++)
		{
			int item = Random.Range(0, num);
			list.Add(item);
			list.Add(item);
		}
		for (int num3 = list.Count - 1; num3 > 0; num3--)
		{
			int index = Random.Range(0, num3 + 1);
			int value = list[num3];
			list[num3] = list[index];
			list[index] = value;
		}
		int num4 = 0;
		for (int k = 0; k < 4; k++)
		{
			for (int l = 0; l < 4; l++)
			{
				mmGrid[k, l] = list[num4++];
			}
		}
	}

	private void DrawCheckers()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Expected O, but got Unknown
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		if (ckBoard == null)
		{
			CKNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Checkers</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), (!ckGameOver) ? ((ckTurn == 1) ? "Your turn (dark)" : "AI thinking...") : ((ckWinner == 1) ? "You Win!" : "AI Wins!"));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			CKNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 38f;
		float num2 = 180f;
		float num3 = 72f;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				GUI.backgroundColor = (((i + j) % 2 == 0) ? new Color(0.3f, 0.3f, 0.35f) : new Color(0.8f, 0.8f, 0.75f));
				if (i == ckSelR && j == ckSelC)
				{
					GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
				}
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "") && !ckGameOver && ckTurn == 1)
				{
					int num6 = ckBoard[i, j];
					if (num6 == 1 || num6 == 3)
					{
						ckSelR = i;
						ckSelC = j;
					}
					else if (ckSelR >= 0 && num6 == 0)
					{
						CKTryMove(i, j);
					}
				}
				int num7 = ckBoard[i, j];
				if (num7 > 0)
				{
					GUI.backgroundColor = ((num7 == 1 || num7 == 3) ? new Color(0.2f, 0.2f, 0.25f) : new Color(0.85f, 0.25f, 0.2f));
					GUI.Button(new Rect(num4 + num * 0.2f, num5 + num * 0.2f, num * 0.6f, num * 0.6f), "");
					if (num7 >= 3)
					{
						GUI.color = Color.yellow;
						GUIStyle val = new GUIStyle(GUI.skin.label)
						{
							fontSize = 10,
							alignment = (TextAnchor)4
						};
						GUI.Label(new Rect(num4 + num * 0.2f, num5 + num * 0.25f, num * 0.6f, num * 0.5f), "★", val);
						GUI.color = Color.white;
					}
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (!ckGameOver && ckTurn == 2)
		{
			ckBoard = CKAIMove(ckBoard);
			int num8 = CKMustJump(ckBoard, 2);
			if (num8 > 0)
			{
				ckBoard = CKAIMove(ckBoard);
			}
			ckTurn = 1;
			CKCheckWin();
		}
	}

	private void CKNewGame()
	{
		ckBoard = new int[8, 8];
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if ((i + j) % 2 == 0)
				{
					if (i < 3)
					{
						ckBoard[i, j] = 2;
					}
					else if (i > 4)
					{
						ckBoard[i, j] = 1;
					}
				}
			}
		}
		ckTurn = 1;
		ckSelR = (ckSelC = -1);
		ckGameOver = false;
		ckWinner = 0;
	}

	private int CKMustJump(int[,] board, int player)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				int num2 = board[i, j];
				if (num2 == 0 || ((player != 1 || (num2 != 1 && num2 != 3)) && (player != 2 || (num2 != 2 && num2 != 4))))
				{
					continue;
				}
				int num3 = ((num2 != 1 && num2 != 3) ? 1 : (-1));
				int[] array = new int[2] { -1, 1 };
				for (int k = 0; k < 2; k++)
				{
					int num4 = i + num3 * 2;
					int num5 = j + array[k] * 2;
					int num6 = i + num3;
					int num7 = j + array[k];
					if (num4 >= 0 && num4 < 8 && num5 >= 0 && num5 < 8 && ((player == 1 && (board[num6, num7] == 2 || board[num6, num7] == 4)) || (player == 2 && (board[num6, num7] == 1 || board[num6, num7] == 3))) && board[num4, num5] == 0)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	private void CKTryMove(int tr, int tc)
	{
		int num = ckSelR;
		int num2 = ckSelC;
		int num3 = ckBoard[num, num2];
		int num4 = ((num3 != 1 && num3 != 3) ? 1 : (-1));
		int num5 = tr - num;
		int value = tc - num2;
		bool flag = Math.Abs(num5) == 2 && Math.Abs(value) == 2 && (num5 == num4 * 2 || num3 == 3);
		bool flag2 = Math.Abs(num5) == 1 && Math.Abs(value) == 1 && num5 == num4;
		int num6 = CKMustJump(ckBoard, 1);
		if (flag && num6 > 0)
		{
			int num7 = (num + tr) / 2;
			int num8 = (num2 + tc) / 2;
			if (ckBoard[num7, num8] == 2 || ckBoard[num7, num8] == 4)
			{
				ckBoard[tr, tc] = num3;
				ckBoard[num, num2] = 0;
				ckBoard[num7, num8] = 0;
				if (tr == 0)
				{
					ckBoard[tr, tc] = 3;
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				ckSelR = (ckSelC = -1);
				ckTurn = 2;
				CKCheckWin();
			}
		}
		else if (flag2 && num6 == 0)
		{
			ckBoard[tr, tc] = num3;
			ckBoard[num, num2] = 0;
			if (tr == 0)
			{
				ckBoard[tr, tc] = 3;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			ckSelR = (ckSelC = -1);
			ckTurn = 2;
			CKCheckWin();
		}
	}

	private int[,] CKAIMove(int[,] board)
	{
		List<int[]> list = new List<int[]>();
		List<int[]> list2 = new List<int[]>();
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				int num = board[i, j];
				if (num != 2 && num != 4)
				{
					continue;
				}
				int num2 = ((num == 2) ? 1 : (-1));
				int[] array = new int[2] { -1, 1 };
				for (int k = 0; k < 2; k++)
				{
					int num3 = i + num2;
					int num4 = j + array[k];
					if (num3 >= 0 && num3 < 8 && num4 >= 0 && num4 < 8 && board[num3, num4] == 0)
					{
						list2.Add(new int[4] { i, j, num3, num4 });
					}
					int num5 = i + num2;
					int num6 = j + array[k];
					int num7 = i + num2 * 2;
					int num8 = j + array[k] * 2;
					if (num7 >= 0 && num7 < 8 && num8 >= 0 && num8 < 8 && (board[num5, num6] == 1 || board[num5, num6] == 3) && board[num7, num8] == 0)
					{
						list.Add(new int[4] { i, j, num7, num8 });
					}
				}
			}
		}
		List<int[]> list3 = ((list.Count > 0) ? list : list2);
		if (list3.Count == 0)
		{
			return board;
		}
		int[] array2 = list3[Random.Range(0, list3.Count)];
		int num9 = board[array2[0], array2[1]];
		board[array2[2], array2[3]] = num9;
		board[array2[0], array2[1]] = 0;
		if (Math.Abs(array2[2] - array2[0]) == 2)
		{
			board[(array2[0] + array2[2]) / 2, (array2[1] + array2[3]) / 2] = 0;
		}
		if (array2[2] == 7)
		{
			board[array2[2], array2[3]] = 4;
		}
		return board;
	}

	private void CKCheckWin()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (ckBoard[i, j] == 1 || ckBoard[i, j] == 3)
				{
					flag = true;
				}
				if (ckBoard[i, j] == 2 || ckBoard[i, j] == 4)
				{
					flag2 = true;
				}
			}
		}
		if (!flag)
		{
			ckGameOver = true;
			ckWinner = 2;
		}
		if (!flag2)
		{
			ckGameOver = true;
			ckWinner = 1;
		}
	}

	private void DrawSudoku()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Invalid comparison between Unknown and I4
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Invalid comparison between Unknown and I4
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Invalid comparison between Unknown and I4
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Invalid comparison between Unknown and I4
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Expected I4, but got Unknown
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Invalid comparison between Unknown and I4
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Expected I4, but got Unknown
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Invalid comparison between Unknown and I4
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Expected O, but got Unknown
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		if (sdkGrid == null)
		{
			SDKNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Sudoku</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Mistakes: {sdkMistakes}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			SDKNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(520f, 46f, 60f, 22f), "Hint"))
		{
			SDKHint();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 34f;
		float num2 = 185f;
		float num3 = 72f;
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				bool flag = i == sdkSelR && j == sdkSelC;
				bool flag2 = sdkFixed[i, j];
				bool flag3 = !flag2 && sdkGrid[i, j] != 0 && sdkGrid[i, j] != sdkSol[i, j];
				GUI.backgroundColor = (flag ? new Color(0.4f, 0.7f, 1f) : (flag2 ? new Color(0.25f, 0.25f, 0.3f) : (flag3 ? new Color(0.7f, 0.25f, 0.25f) : new Color(0.35f, 0.35f, 0.4f))));
				string text = ((sdkGrid[i, j] > 0) ? sdkGrid[i, j].ToString() : "");
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), text) && !flag2)
				{
					sdkSelR = i;
					sdkSelC = j;
				}
				if ((j + 1) % 3 == 0 && j < 8)
				{
					GUI.backgroundColor = Color.white;
					GUI.Button(new Rect(num4 + num - 1f, num5, 2f, num - 1f), "");
				}
			}
		}
		for (int k = 0; k < 3; k++)
		{
			GUI.backgroundColor = Color.white;
			GUI.Button(new Rect(num2, num3 + (float)(k * 3) * num - 1f, 9f * num, 2f), "");
		}
		GUI.backgroundColor = guiColorB;
		Event current = Event.current;
		if ((int)current.type != 4 || sdkSelR < 0)
		{
			return;
		}
		int num6 = 0;
		if ((int)current.keyCode >= 49 && (int)current.keyCode <= 57)
		{
			num6 = current.keyCode - 49 + 1;
		}
		if ((int)current.keyCode >= 257 && (int)current.keyCode <= 265)
		{
			num6 = current.keyCode - 257 + 1;
		}
		if (num6 > 0 && !sdkFixed[sdkSelR, sdkSelC])
		{
			sdkGrid[sdkSelR, sdkSelC] = num6;
			if (num6 != sdkSol[sdkSelR, sdkSelC])
			{
				sdkMistakes++;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			bool flag4 = true;
			for (int l = 0; l < 9; l++)
			{
				for (int m = 0; m < 9; m++)
				{
					if (sdkGrid[l, m] != sdkSol[l, m])
					{
						flag4 = false;
					}
				}
			}
			if (flag4)
			{
				GUI.color = Color.green;
				GUIStyle val = new GUIStyle(GUI.skin.label)
				{
					fontSize = 16,
					fontStyle = (FontStyle)1,
					alignment = (TextAnchor)4
				};
				GUI.Label(new Rect(num2, num3 + 9f * num + 5f, 9f * num, 25f), "Solved!", val);
				GUI.color = Color.white;
			}
			current.Use();
		}
		if ((int)current.keyCode == 8 && sdkSelR >= 0 && !sdkFixed[sdkSelR, sdkSelC])
		{
			sdkGrid[sdkSelR, sdkSelC] = 0;
			current.Use();
		}
	}

	private void SDKNewGame()
	{
		sdkSol = new int[9, 9];
		sdkGrid = new int[9, 9];
		sdkFixed = new bool[9, 9];
		int[] array = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		for (int num = 8; num > 0; num--)
		{
			int num2 = Random.Range(0, num + 1);
			int num3 = array[num];
			array[num] = array[num2];
			array[num2] = num3;
		}
		for (int i = 0; i < 9; i++)
		{
			sdkSol[0, i] = array[i];
		}
		SDKFill(sdkSol);
		for (int j = 0; j < 9; j++)
		{
			for (int k = 0; k < 9; k++)
			{
				sdkGrid[j, k] = sdkSol[j, k];
				sdkFixed[j, k] = true;
			}
		}
		int num4 = 40;
		while (num4 > 0)
		{
			int num5 = Random.Range(0, 9);
			int num6 = Random.Range(0, 9);
			if (sdkFixed[num5, num6])
			{
				sdkFixed[num5, num6] = false;
				sdkGrid[num5, num6] = 0;
				num4--;
			}
		}
		sdkMistakes = 0;
		sdkSelR = (sdkSelC = -1);
	}

	private bool SDKFill(int[,] grid)
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (grid[i, j] != 0)
				{
					continue;
				}
				bool[] array = new bool[10];
				for (int k = 0; k < 9; k++)
				{
					if (grid[i, k] > 0)
					{
						array[grid[i, k]] = true;
					}
					if (grid[k, j] > 0)
					{
						array[grid[k, j]] = true;
					}
				}
				int num = i / 3 * 3;
				int num2 = j / 3 * 3;
				for (int l = 0; l < 3; l++)
				{
					for (int m = 0; m < 3; m++)
					{
						if (grid[num + l, num2 + m] > 0)
						{
							array[grid[num + l, num2 + m]] = true;
						}
					}
				}
				List<int> list = new List<int>();
				for (int n = 1; n <= 9; n++)
				{
					if (!array[n])
					{
						list.Add(n);
					}
				}
				for (int num3 = list.Count - 1; num3 > 0; num3--)
				{
					int index = Random.Range(0, num3 + 1);
					int value = list[num3];
					list[num3] = list[index];
					list[index] = value;
				}
				foreach (int item in list)
				{
					grid[i, j] = item;
					if (SDKFill(grid))
					{
						return true;
					}
					grid[i, j] = 0;
				}
				return false;
			}
		}
		return true;
	}

	private void SDKHint()
	{
		List<int[]> list = new List<int[]>();
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (sdkGrid[i, j] == 0 || sdkGrid[i, j] != sdkSol[i, j])
				{
					list.Add(new int[2] { i, j });
				}
			}
		}
		if (list.Count != 0)
		{
			int[] array = list[Random.Range(0, list.Count)];
			sdkGrid[array[0], array[1]] = sdkSol[array[0], array[1]];
			sdkFixed[array[0], array[1]] = true;
		}
	}

	private void DrawTowerDefense()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6d: Expected O, but got Unknown
		//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
		if (tdMap == null)
		{
			TDNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Tower Defense</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Wave: {tdWave}  Lives: {tdLives}  Gold: {tdGold}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			TDNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		tdGoldTimer -= Time.deltaTime;
		if (tdGoldTimer <= 0f)
		{
			tdGold += 2;
			tdGoldTimer = 2f;
		}
		float num = 28f;
		float num2 = 180f;
		float num3 = 72f;
		for (int i = 0; i < 12; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				float num6 = tdMap[i, j, 0];
				GUI.backgroundColor = ((num6 == 0f) ? new Color(0.15f, 0.55f, 0.15f) : ((num6 == 1f) ? new Color(0.6f, 0.5f, 0.3f) : ((num6 == 2f) ? new Color(0.3f, 0.3f, 0.35f) : ((num6 == 3f) ? new Color(0.85f, 0.2f, 0.2f) : new Color(0.5f, 0.5f, 0.8f)))));
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "") && num6 == 0f && tdSelTow >= 0)
				{
					float num7 = tdTowCost[tdSelTow];
					if ((float)tdGold >= num7)
					{
						tdGold -= (int)num7;
						tdMap[i, j, 0] = 2f;
						tdMap[i, j, 1] = tdSelTow;
						tdMap[i, j, 2] = 0f;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
				}
			}
		}
		for (int k = 0; k < tdEnemies.Count; k++)
		{
			float num8 = num2 + tdEnemies[k].x * num;
			float num9 = num3 + tdEnemies[k].y * num;
			GUI.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
			GUI.Button(new Rect(num8 + 2f, num9 + 2f, num - 5f, num - 5f), "");
			float num10 = tdEnemyHP[k] / tdEnemyMaxHP[k];
			GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
			GUI.Button(new Rect(num8 + 2f, num9 - 3f, (num - 5f) * num10, 3f), "");
		}
		for (int l = 0; l < 12; l++)
		{
			for (int m = 0; m < 16; m++)
			{
				if (tdMap[l, m, 0] != 2f)
				{
					continue;
				}
				int num11 = (int)tdMap[l, m, 1];
				float num12 = tdTowRange[num11];
				float num13 = num12 / num;
				GUI.color = new Color(1f, 1f, 1f, 0.15f);
				GUI.backgroundColor = Color.clear;
				GUI.Button(new Rect(num2 + ((float)m - num13) * num, num3 + ((float)l - num13) * num, num13 * 2f * num, num13 * 2f * num), "");
				GUI.color = Color.white;
				tdMap[l, m, 2] += Time.deltaTime;
				if (!(tdMap[l, m, 2] >= tdTowRate[num11]))
				{
					continue;
				}
				int num14 = -1;
				float num15 = float.MaxValue;
				for (int n = 0; n < tdEnemies.Count; n++)
				{
					float num16 = tdEnemies[n].x - (float)m;
					float num17 = tdEnemies[n].y - (float)l;
					float num18 = Mathf.Sqrt(num16 * num16 + num17 * num17);
					if (num18 <= num13 && num18 < num15)
					{
						num15 = num18;
						num14 = n;
					}
				}
				if (num14 >= 0)
				{
					tdEnemyHP[num14] -= tdTowDmg[num11];
					tdMap[l, m, 2] = 0f;
				}
			}
		}
		for (int num19 = tdEnemies.Count - 1; num19 >= 0; num19--)
		{
			if (tdEnemyHP[num19] <= 0f)
			{
				tdEnemies.RemoveAt(num19);
				tdEnemyHP.RemoveAt(num19);
				tdEnemyMaxHP.RemoveAt(num19);
				continue;
			}
			Vector4 val = tdEnemies[num19];
			float num20 = 1.5f * Time.deltaTime;
			switch ((int)val.z)
			{
			case 0:
				val.x += num20;
				if (val.x >= 15f)
				{
					val.z = 1f;
					val.y += 1f;
				}
				break;
			case 1:
				val.y += num20;
				if (val.y >= 11f)
				{
					val.z = 2f;
					val.x -= 1f;
				}
				break;
			default:
				val.x -= num20;
				if (val.x <= 0f)
				{
					tdEnemies.RemoveAt(num19);
					tdEnemyHP.RemoveAt(num19);
					tdEnemyMaxHP.RemoveAt(num19);
					tdLives--;
					if (tdLives <= 0)
					{
						tdActive = false;
					}
				}
				continue;
			}
			tdEnemies[num19] = val;
		}
		if (tdActive && tdSpawned < tdWaveEnemies)
		{
			tdSpawnTimer -= Time.deltaTime;
			if (tdSpawnTimer <= 0f)
			{
				float item = 30f + (float)tdWave * 15f;
				tdEnemies.Add(new Vector4(0f, 1f, 0f, 0f));
				tdEnemyHP.Add(item);
				tdEnemyMaxHP.Add(item);
				tdSpawned++;
				tdSpawnTimer = 1f;
			}
		}
		if (tdActive && tdSpawned >= tdWaveEnemies && tdEnemies.Count == 0)
		{
			tdWave++;
			tdWaveEnemies = 5 + tdWave * 2;
			tdSpawned = 0;
			tdGold += 25;
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num2, num3 + 12f * num + 5f, 16f * num, 20f), "<size=11>Click tower type below, then click a grass tile to place. Towers auto-shoot nearest enemy.</size>");
		GUI.backgroundColor = guiColorA;
		for (int num21 = 0; num21 < 3; num21++)
		{
			GUI.backgroundColor = (Color)((tdSelTow == num21) ? new Color(0.4f, 0.8f, 0.4f) : guiColorA);
			if (GUI.Button(new Rect(num2 + (float)num21 * 140f, num3 + 12f * num + 25f, 130f, 22f), tdTowNames[num21]))
			{
				tdSelTow = num21;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		if (!tdActive && tdLives <= 0)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num2, num3 + 12f * num + 55f, 16f * num, 25f), "Game Over! All lives lost.", val2);
			GUI.color = Color.white;
		}
	}

	private void TDNewGame()
	{
		tdMap = new float[12, 16, 3];
		for (int i = 1; i < 15; i++)
		{
			tdMap[1, i, 0] = 1f;
		}
		tdMap[1, 15, 0] = 1f;
		tdMap[1, 15, 2] = 0f;
		for (int j = 1; j < 10; j++)
		{
			tdMap[j, 15, 0] = 1f;
		}
		for (int k = 1; k < 16; k++)
		{
			tdMap[10, k, 0] = 1f;
		}
		tdMap[0, 0, 0] = 3f;
		tdEnemies = new List<Vector4>();
		tdEnemyHP = new List<float>();
		tdEnemyMaxHP = new List<float>();
		tdWave = 1;
		tdLives = 10;
		tdGold = 100;
		tdSelTow = 0;
		tdActive = true;
		tdSpawned = 0;
		tdWaveEnemies = 7;
		tdSpawnTimer = 0f;
		tdGoldTimer = 2f;
	}

	private void DrawMaze()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Invalid comparison between Unknown and I4
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Invalid comparison between Unknown and I4
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Invalid comparison between Unknown and I4
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Invalid comparison between Unknown and I4
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Invalid comparison between Unknown and I4
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Invalid comparison between Unknown and I4
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Invalid comparison between Unknown and I4
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Invalid comparison between Unknown and I4
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Invalid comparison between Unknown and I4
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Invalid comparison between Unknown and I4
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Invalid comparison between Unknown and I4
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Invalid comparison between Unknown and I4
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Invalid comparison between Unknown and I4
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Invalid comparison between Unknown and I4
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Invalid comparison between Unknown and I4
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Invalid comparison between Unknown and I4
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Invalid comparison between Unknown and I4
		if (!mzGenerated)
		{
			MazeGenerate();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Maze</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), mzDone ? "You escaped! Generate a new maze." : "Reach the green exit!");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Maze"))
		{
			MazeGenerate();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		Event current = Event.current;
		if ((int)current.type == 4 && !mzDone)
		{
			int[] array = new int[4] { -1, 0, 1, 0 };
			int[] array2 = new int[4] { 0, 1, 0, -1 };
			int[] array3 = new int[4] { 1, 2, 4, 8 };
			int[] array4 = new int[4] { 4, 8, 1, 2 };
			for (int i = 0; i < 4; i++)
			{
				if ((((int)current.keyCode == 273 || (int)current.keyCode == 119) && i != 0) || (((int)current.keyCode == 274 || (int)current.keyCode == 115) && i != 2) || (((int)current.keyCode == 276 || (int)current.keyCode == 97) && i != 3) || (((int)current.keyCode == 275 || (int)current.keyCode == 100) && i != 1) || ((int)current.keyCode != 273 && (int)current.keyCode != 119 && (int)current.keyCode != 274 && (int)current.keyCode != 115 && (int)current.keyCode != 276 && (int)current.keyCode != 97 && (int)current.keyCode != 275 && (int)current.keyCode != 100))
				{
					continue;
				}
				int num = mzPR + array[i];
				int num2 = mzPC + array2[i];
				if (num >= 0 && num < mzH && num2 >= 0 && num2 < mzW && (mzWalls[mzPR, mzPC] & array3[i]) == 0)
				{
					mzPR = num;
					mzPC = num2;
					if (mzPR == mzER && mzPC == mzEC)
					{
						mzDone = true;
					}
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				current.Use();
				break;
			}
		}
		float num3 = 22f;
		float num4 = 180f;
		float num5 = 72f;
		for (int j = 0; j < mzH; j++)
		{
			for (int k = 0; k < mzW; k++)
			{
				float num6 = num4 + (float)k * num3;
				float num7 = num5 + (float)j * num3;
				GUI.backgroundColor = new Color(0.18f, 0.18f, 0.22f);
				GUI.Button(new Rect(num6, num7, num3 - 1f, num3 - 1f), "");
				int num8 = mzWalls[j, k];
				GUI.backgroundColor = Color.clear;
				if ((num8 & 1) != 0)
				{
					GUI.Button(new Rect(num6, num7, num3 - 1f, 2f), "");
				}
				if ((num8 & 2) != 0)
				{
					GUI.Button(new Rect(num6 + num3 - 3f, num7, 2f, num3 - 1f), "");
				}
				if ((num8 & 4) != 0)
				{
					GUI.Button(new Rect(num6, num7 + num3 - 3f, num3 - 1f, 2f), "");
				}
				if ((num8 & 8) != 0)
				{
					GUI.Button(new Rect(num6, num7, 2f, num3 - 1f), "");
				}
			}
		}
		GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
		GUI.Button(new Rect(num4 + (float)mzEC * num3 + 3f, num5 + (float)mzER * num3 + 3f, num3 - 7f, num3 - 7f), "");
		GUI.backgroundColor = new Color(0.3f, 0.5f, 0.9f);
		GUI.Button(new Rect(num4 + (float)mzPC * num3 + 3f, num5 + (float)mzPR * num3 + 3f, num3 - 7f, num3 - 7f), "");
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num4, num5 + (float)mzH * num3 + 5f, (float)mzW * num3, 20f), "<size=11>Arrow keys or WASD to move</size>");
	}

	private void MazeGenerate()
	{
		mzWalls = new int[mzH, mzW];
		for (int i = 0; i < mzH; i++)
		{
			for (int j = 0; j < mzW; j++)
			{
				mzWalls[i, j] = 15;
			}
		}
		bool[,] array = new bool[mzH, mzW];
		Stack<int[]> stack = new Stack<int[]>();
		stack.Push(new int[2]);
		array[0, 0] = true;
		int[] array2 = new int[4] { -1, 0, 1, 0 };
		int[] array3 = new int[4] { 0, 1, 0, -1 };
		int[] array4 = new int[4] { 1, 2, 4, 8 };
		int[] array5 = new int[4] { 4, 8, 1, 2 };
		while (stack.Count > 0)
		{
			int[] array6 = stack.Peek();
			List<int> list = new List<int>();
			for (int k = 0; k < 4; k++)
			{
				int num = array6[0] + array2[k];
				int num2 = array6[1] + array3[k];
				if (num >= 0 && num < mzH && num2 >= 0 && num2 < mzW && !array[num, num2])
				{
					list.Add(k);
				}
			}
			if (list.Count == 0)
			{
				stack.Pop();
				continue;
			}
			int num3 = list[Random.Range(0, list.Count)];
			int num4 = array6[0] + array2[num3];
			int num5 = array6[1] + array3[num3];
			mzWalls[array6[0], array6[1]] &= ~array4[num3];
			mzWalls[num4, num5] &= ~array5[num3];
			array[num4, num5] = true;
			stack.Push(new int[2] { num4, num5 });
		}
		mzPR = 0;
		mzPC = 0;
		mzER = mzH - 1;
		mzEC = mzW - 1;
		mzDone = false;
		mzGenerated = true;
	}

	private void DrawBreakout()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Invalid comparison between Unknown and I4
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Invalid comparison between Unknown and I4
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Invalid comparison between Unknown and I4
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Invalid comparison between Unknown and I4
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Invalid comparison between Unknown and I4
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Invalid comparison between Unknown and I4
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Expected O, but got Unknown
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Expected O, but got Unknown
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		if (brBricks == null)
		{
			BRNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Breakout</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {brScore}  Lives: {brLives}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			BRNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 190f;
		float num2 = 72f;
		Event current = Event.current;
		if ((int)current.type == 0)
		{
			brPaddleX = Mathf.Clamp(current.mousePosition.x - num - 25f, 0f, brFieldW - 50f);
		}
		if ((int)current.type == 4)
		{
			if ((int)current.keyCode == 276 || (int)current.keyCode == 97)
			{
				brPaddleX = Mathf.Max(0f, brPaddleX - 20f);
			}
			if ((int)current.keyCode == 275 || (int)current.keyCode == 100)
			{
				brPaddleX = Mathf.Min(brFieldW - 50f, brPaddleX + 20f);
			}
		}
		if (brActive)
		{
			float deltaTime = Time.deltaTime;
			brBallX += brBallVX * deltaTime;
			brBallY += brBallVY * deltaTime;
			if (brBallX <= 0f || brBallX + 8f >= brFieldW)
			{
				brBallVX = 0f - brBallVX;
			}
			if (brBallY <= 0f)
			{
				brBallVY = 0f - brBallVY;
			}
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(num + brBallX, num2 + brBallY, 8f, 8f);
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(num + brPaddleX, num2 + brFieldH - 15f, 50f, 10f);
			if (((Rect)(ref val)).Overlaps(val2) && brBallVY > 0f)
			{
				float num3 = (brBallX + 4f - (brPaddleX + 25f)) / 25f;
				float num4 = Mathf.Sqrt(brBallVX * brBallVX + brBallVY * brBallVY);
				brBallVX = num3 * num4;
				brBallVY = 0f - Mathf.Abs(num4 * 0.9f);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			Rect val3 = default(Rect);
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					if (brBricks[i, j])
					{
						((Rect)(ref val3))._002Ector(num + (float)j * 38f + 2f, num2 + 80f + (float)i * 16f, 36f, 14f);
						if (((Rect)(ref val)).Overlaps(val3))
						{
							brBricks[i, j] = false;
							brBallVY = 0f - brBallVY;
							brScore += 10;
							SoundManager.Play(SoundManager.DefaultSounds["Button"]);
							break;
						}
					}
				}
			}
			if (brBallY > brFieldH)
			{
				brLives--;
				if (brLives <= 0)
				{
					brActive = false;
				}
				else
				{
					BRResetBall();
				}
			}
			bool flag = true;
			for (int k = 0; k < 5; k++)
			{
				for (int l = 0; l < 10; l++)
				{
					if (brBricks[k, l])
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				brActive = false;
				brScore += 100;
			}
		}
		GUI.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
		GUI.Box(new Rect(num, num2, brFieldW, brFieldH), "");
		for (int m = 0; m < 5; m++)
		{
			for (int n = 0; n < 10; n++)
			{
				if (brBricks[m, n])
				{
					Color backgroundColor = (new Color[5]
					{
						Color.red,
						Color.orange,
						Color.yellow,
						Color.green,
						Color.cyan
					})[m];
					GUI.backgroundColor = backgroundColor;
					GUI.Box(new Rect(num + (float)n * 38f + 2f, num2 + 80f + (float)m * 16f, 36f, 14f), "");
				}
			}
		}
		GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
		GUI.Box(new Rect(num + brPaddleX, num2 + brFieldH - 15f, 50f, 10f), "");
		GUI.backgroundColor = Color.yellow;
		GUI.Box(new Rect(num + brBallX, num2 + brBallY, 8f, 8f), "");
		GUI.backgroundColor = guiColorB;
		if (!brActive && brLives > 0)
		{
			GUI.color = Color.green;
			GUIStyle val4 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + brFieldH / 2f - 15f, brFieldW, 30f), "You Win!", val4);
			GUI.color = Color.white;
		}
		else if (!brActive && brLives <= 0)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val5 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + brFieldH / 2f - 15f, brFieldW, 30f), "Game Over!", val5);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num, num2 + brFieldH + 5f, brFieldW, 16f), "<size=11>Mouse or A/D arrows to move paddle</size>");
	}

	private void BRNewGame()
	{
		brBricks = new bool[5, 10];
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				brBricks[i, j] = true;
			}
		}
		brPaddleX = brFieldW / 2f - 25f;
		brScore = 0;
		brLives = 3;
		brActive = true;
		BRResetBall();
	}

	private void BRResetBall()
	{
		brBallX = brFieldW / 2f - 4f;
		brBallY = brFieldH - 40f;
		brBallVX = Random.Range(-100f, 100f);
		brBallVY = -200f;
	}

	private void DrawMSHard()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Expected O, but got Unknown
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		if (mshGrid == null)
		{
			MSHNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Minesweeper Hard</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Mines: {mshMines}  Flags: {mshFlagsLeft}  Time: {mshTimer:F1}s");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 90f, 22f), mshFlagMode ? "⛏ Dig" : "\ud83d\udea9 Flag"))
		{
			mshFlagMode = !mshFlagMode;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (GUI.Button(new Rect(530f, 46f, 80f, 22f), "New Game"))
		{
			MSHNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (!mshGameOver && mshStarted)
		{
			mshTimer += Time.deltaTime;
		}
		float num = 22f;
		float num2 = 180f;
		float num3 = 72f;
		mshScrollPos = GUI.BeginScrollView(new Rect(170f, 68f, 530f, 340f), mshScrollPos, new Rect(0f, 0f, (float)mshCols * num + 10f, (float)mshRows * num + 10f));
		for (int i = 0; i < mshRows; i++)
		{
			for (int j = 0; j < mshCols; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				if (mshRevealed[i, j])
				{
					if (mshGrid[i, j] == -1)
					{
						GUI.backgroundColor = new Color(0.9f, 0.2f, 0.2f);
						GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "\ud83d\udca3");
					}
					else
					{
						GUI.backgroundColor = new Color(0.75f, 0.75f, 0.8f);
						int num6 = mshGrid[i, j];
						GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), (num6 > 0) ? num6.ToString() : "");
					}
					continue;
				}
				if (mshFlagged[i, j])
				{
					GUI.backgroundColor = new Color(1f, 0.85f, 0.4f);
					if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "\ud83d\udea9") && !mshGameOver)
					{
						mshFlagged[i, j] = false;
						mshFlagsLeft++;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					continue;
				}
				GUI.backgroundColor = new Color(0.55f, 0.6f, 0.65f);
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "") && !mshGameOver)
				{
					if (mshFlagMode)
					{
						mshFlagged[i, j] = true;
						mshFlagsLeft--;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					else
					{
						MSHReveal(i, j);
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
				}
			}
		}
		GUI.EndScrollView();
		GUI.backgroundColor = guiColorB;
		if (mshGameOver)
		{
			GUI.color = (Color)(mshWon ? Color.green : new Color(1f, 0.4f, 0.4f));
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(180f, num3 + (float)mshRows * 22f + 10f, (float)mshCols * 22f, 25f), mshWon ? "You Win!" : "Game Over!", val);
			GUI.color = Color.white;
		}
	}

	private void MSHNewGame()
	{
		mshGrid = new int[mshRows, mshCols];
		mshRevealed = new bool[mshRows, mshCols];
		mshFlagged = new bool[mshRows, mshCols];
		int num = 0;
		while (num < mshMines)
		{
			int num2 = Random.Range(0, mshRows);
			int num3 = Random.Range(0, mshCols);
			if (mshGrid[num2, num3] != -1)
			{
				mshGrid[num2, num3] = -1;
				num++;
			}
		}
		for (int i = 0; i < mshRows; i++)
		{
			for (int j = 0; j < mshCols; j++)
			{
				if (mshGrid[i, j] == -1)
				{
					continue;
				}
				int num4 = 0;
				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						int num5 = i + k;
						int num6 = j + l;
						if (num5 >= 0 && num5 < mshRows && num6 >= 0 && num6 < mshCols && mshGrid[num5, num6] == -1)
						{
							num4++;
						}
					}
				}
				mshGrid[i, j] = num4;
			}
		}
		mshFlagsLeft = mshMines;
		mshGameOver = false;
		mshWon = false;
		mshStarted = false;
		mshTimer = 0f;
		mshFlagMode = false;
	}

	private void MSHReveal(int r, int c)
	{
		if (r < 0 || r >= mshRows || c < 0 || c >= mshCols || mshRevealed[r, c] || mshFlagged[r, c])
		{
			return;
		}
		mshRevealed[r, c] = true;
		mshStarted = true;
		if (mshGrid[r, c] == -1)
		{
			mshGameOver = true;
			for (int i = 0; i < mshRows; i++)
			{
				for (int j = 0; j < mshCols; j++)
				{
					if (mshGrid[i, j] == -1)
					{
						mshRevealed[i, j] = true;
					}
				}
			}
			return;
		}
		if (mshGrid[r, c] == 0)
		{
			for (int k = -1; k <= 1; k++)
			{
				for (int l = -1; l <= 1; l++)
				{
					MSHReveal(r + k, c + l);
				}
			}
		}
		int num = 0;
		for (int m = 0; m < mshRows; m++)
		{
			for (int n = 0; n < mshCols; n++)
			{
				if (!mshRevealed[m, n] && mshGrid[m, n] != -1)
				{
					num++;
				}
			}
		}
		if (num == 0)
		{
			mshGameOver = true;
			mshWon = true;
		}
	}

	private void DrawChineseCheckers()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Chinese Checkers</size>");
		GUI.Label(new Rect(170f, 48f, 500f, 20f), ccGameOver ? "<size=12>Congratulations! Click New Game to play again</size>" : $"<size=12>Moves: {ccMoves}  |  Click a green piece, then click destination</size>");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			CCNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (ccBoard == null)
		{
			CCNewGame();
		}
		float num = 32f;
		float num2 = 210f;
		float num3 = 72f;
		Color backgroundColor = default(Color);
		for (int i = 0; i < ccBoardSize; i++)
		{
			for (int j = 0; j < ccBoardSize; j++)
			{
				if (!CCIsValid(i, j))
				{
					continue;
				}
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				int num6 = ccBoard[i, j];
				switch (num6)
				{
				case 1:
					((Color)(ref backgroundColor))._002Ector(0.2f, 0.7f, 0.3f);
					break;
				case 2:
					((Color)(ref backgroundColor))._002Ector(0.85f, 0.25f, 0.25f);
					break;
				case 3:
					((Color)(ref backgroundColor))._002Ector(0.2f, 0.2f, 0.85f);
					break;
				default:
					((Color)(ref backgroundColor))._002Ector(0.3f, 0.3f, 0.35f);
					break;
				}
				if (i == ccSelR && j == ccSelC)
				{
					((Color)(ref backgroundColor))._002Ector(1f, 1f, 0.3f);
				}
				GUI.backgroundColor = backgroundColor;
				if (!GUI.Button(new Rect(num4, num5, num - 2f, num - 2f), num6 switch
				{
					3 => "B", 
					2 => "R", 
					1 => "G", 
					_ => "", 
				}) || ccGameOver)
				{
					continue;
				}
				if (num6 == 1)
				{
					ccSelR = i;
					ccSelC = j;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				else
				{
					if (ccSelR < 0)
					{
						continue;
					}
					bool flag = false;
					if (num6 == 0 && CCIsAdjacent(ccSelR, ccSelC, i, j))
					{
						ccBoard[i, j] = 1;
						ccBoard[ccSelR, ccSelC] = 0;
						flag = true;
					}
					else if (num6 != 0 && CCIsAdjacent(ccSelR, ccSelC, i, j))
					{
						int num7 = i + (i - ccSelR);
						int num8 = j + (j - ccSelC);
						if (CCIsValid(num7, num8) && ccBoard[num7, num8] == 0)
						{
							ccBoard[num7, num8] = 1;
							ccBoard[ccSelR, ccSelC] = 0;
							flag = true;
							i = num7;
							j = num8;
						}
					}
					if (flag)
					{
						ccSelR = (ccSelC = -1);
						ccMoves++;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
						CCPlayerMoved();
					}
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num2, num3 + (float)ccBoardSize * num + 8f, (float)ccBoardSize * num, 20f), "<size=11>Green=You  Red=AI  Blue=Obstacles  |  Click green, then click empty neighbor</size>");
	}

	private bool CCIsValid(int r, int c)
	{
		int num = ccBoardSize / 2;
		if (r < 0 || r >= ccBoardSize || c < 0 || c >= ccBoardSize)
		{
			return false;
		}
		int num2 = Mathf.Abs(r - num);
		int num3 = Mathf.Abs(c - num);
		return num2 + num3 <= num + 1;
	}

	private bool CCIsAdjacent(int r1, int c1, int r2, int c2)
	{
		int num = Mathf.Abs(r1 - r2);
		int num2 = Mathf.Abs(c1 - c2);
		return num <= 1 && num2 <= 1 && num + num2 > 0;
	}

	private void CCNewGame()
	{
		ccBoard = new int[ccBoardSize, ccBoardSize];
		int num = ccBoardSize / 2;
		for (int i = 0; i < ccBoardSize; i++)
		{
			for (int j = 0; j < ccBoardSize; j++)
			{
				ccBoard[i, j] = 0;
			}
		}
		for (int k = 0; k < 3; k++)
		{
			ccBoard[ccBoardSize - 1, num - 1 + k] = 1;
			ccBoard[0, num - 1 + k] = 2;
			if (k < 2)
			{
				ccBoard[ccBoardSize - 2, num + k] = 3;
				ccBoard[1, num - 1 + k] = 3;
			}
		}
		ccPlayerPieces = 3;
		ccAIPieces = 3;
		ccSelR = (ccSelC = -1);
		ccMoves = 0;
		ccGameOver = false;
	}

	private bool CCCanJump(int r1, int c1, int r2, int c2, int lr, int lc)
	{
		int num = (r1 + r2) / 2;
		int num2 = (c1 + c2) / 2;
		return ccBoard[num, num2] != 0 && ccBoard[lr, lc] == 0 && CCIsValid(lr, lc);
	}

	private void CCPlayerMoved()
	{
		int num = ccBoardSize / 2;
		int num2 = 0;
		for (int i = num - 1; i <= num + 1; i++)
		{
			if (ccBoard[0, i] == 1)
			{
				num2++;
			}
		}
		if (num2 >= 3)
		{
			ccGameOver = true;
		}
		else
		{
			CCAIMove();
		}
	}

	private void CCAIMove()
	{
		int num = ccBoardSize / 2;
		List<int[]> list = new List<int[]>();
		for (int i = 0; i < ccBoardSize; i++)
		{
			for (int j = 0; j < ccBoardSize; j++)
			{
				if (ccBoard[i, j] == 2)
				{
					list.Add(new int[2] { i, j });
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		int[] array = list[0];
		int num2 = 999;
		int num3 = -1;
		int num4 = -1;
		foreach (int[] item in list)
		{
			int[] array2 = new int[3] { -1, 0, 1 };
			int[] array3 = array2;
			foreach (int num5 in array3)
			{
				int[] array4 = array2;
				foreach (int num6 in array4)
				{
					if (num5 == 0 && num6 == 0)
					{
						continue;
					}
					int num7 = item[0] + num5;
					int num8 = item[1] + num6;
					if (CCIsValid(num7, num8) && ccBoard[num7, num8] == 0)
					{
						int num9 = Mathf.Abs(num7 - (ccBoardSize - 1)) + Mathf.Abs(num8 - num);
						if (num9 < num2)
						{
							num2 = num9;
							array = item;
							num3 = num7;
							num4 = num8;
						}
					}
				}
			}
			for (int m = -2; m <= 2; m++)
			{
				for (int n = -2; n <= 2; n++)
				{
					if ((m == 0 && n == 0) || (Mathf.Abs(m) <= 1 && Mathf.Abs(n) <= 1))
					{
						continue;
					}
					int num10 = item[0] + m;
					int num11 = item[1] + n;
					int num12 = item[0] + m / 2;
					int num13 = item[1] + n / 2;
					if (CCIsValid(num10, num11) && ccBoard[num10, num11] == 0 && CCIsValid(num12, num13) && ccBoard[num12, num13] != 0)
					{
						int num14 = Mathf.Abs(num10 - (ccBoardSize - 1)) + Mathf.Abs(num11 - num);
						if (num14 < num2)
						{
							num2 = num14;
							array = item;
							num3 = num10;
							num4 = num11;
						}
					}
				}
			}
		}
		if (num3 < 0)
		{
			return;
		}
		ccBoard[num3, num4] = 2;
		ccBoard[array[0], array[1]] = 0;
		int num15 = 0;
		for (int num16 = num - 1; num16 <= num + 1; num16++)
		{
			if (ccBoard[ccBoardSize - 1, num16] == 2)
			{
				num15++;
			}
		}
		if (num15 >= 3)
		{
			ccGameOver = true;
		}
	}

	private void DrawTetris()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Invalid comparison between Unknown and I4
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Invalid comparison between Unknown and I4
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Invalid comparison between Unknown and I4
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Invalid comparison between Unknown and I4
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Invalid comparison between Unknown and I4
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Invalid comparison between Unknown and I4
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Expected O, but got Unknown
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Invalid comparison between Unknown and I4
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Invalid comparison between Unknown and I4
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Invalid comparison between Unknown and I4
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		if (tetGrid == null)
		{
			TetNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Tetris</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {tetScore}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			TetNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 22f;
		float num2 = 240f;
		float num3 = 72f;
		Event current = Event.current;
		if ((int)current.type == 4 && tetActive)
		{
			if ((int)current.keyCode == 276 || (int)current.keyCode == 97)
			{
				if (TetCanMove(tetX - 1, tetY, tetPiece, tetRot))
				{
					tetX--;
				}
				current.Use();
			}
			else if ((int)current.keyCode == 275 || (int)current.keyCode == 100)
			{
				if (TetCanMove(tetX + 1, tetY, tetPiece, tetRot))
				{
					tetX++;
				}
				current.Use();
			}
			else if ((int)current.keyCode == 273 || (int)current.keyCode == 119)
			{
				int rot = (tetRot + 1) % 4;
				if (TetCanMove(tetX, tetY, tetPiece, rot))
				{
					tetRot = rot;
				}
				current.Use();
			}
			else if ((int)current.keyCode == 274 || (int)current.keyCode == 115)
			{
				while (TetCanMove(tetX, tetY + 1, tetPiece, tetRot))
				{
					tetY++;
				}
				TetPlace();
				current.Use();
			}
		}
		if (tetActive)
		{
			tetTimer += Time.deltaTime;
			float num4 = Mathf.Max(0.1f, 0.5f - (float)tetScore * 0.002f);
			if (tetTimer >= num4)
			{
				tetTimer = 0f;
				if (TetCanMove(tetX, tetY + 1, tetPiece, tetRot))
				{
					tetY++;
				}
				else
				{
					TetPlace();
				}
			}
		}
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				if (tetGrid[i, j] >= 0)
				{
					GUI.backgroundColor = tetColors[tetGrid[i, j]];
					GUI.Box(new Rect(num2 + (float)j * num, num3 + (float)i * num, num - 1f, num - 1f), "");
				}
				else
				{
					GUI.backgroundColor = new Color(0.15f, 0.15f, 0.2f);
					GUI.Box(new Rect(num2 + (float)j * num, num3 + (float)i * num, num - 1f, num - 1f), "");
				}
			}
		}
		int[,] array = tetShapes[tetPiece];
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		GUI.backgroundColor = tetColors[tetPiece];
		for (int k = 0; k < length; k++)
		{
			for (int l = 0; l < length2; l++)
			{
				if (array[k, l] == 1)
				{
					GUI.Box(new Rect(num2 + (float)(tetX + l) * num, num3 + (float)(tetY + k) * num, num - 1f, num - 1f), "");
				}
			}
		}
		if (tetNext >= 0)
		{
			GUI.Label(new Rect(num2 + 10f * num + 10f, num3, 80f, 20f), "Next:");
			int[,] array2 = tetShapes[tetNext];
			GUI.backgroundColor = tetColors[tetNext];
			for (int m = 0; m < array2.GetLength(0); m++)
			{
				for (int n = 0; n < array2.GetLength(1); n++)
				{
					if (array2[m, n] == 1)
					{
						GUI.Box(new Rect(num2 + 10f * num + 10f + (float)n * num, num3 + 22f + (float)m * num, num - 1f, num - 1f), "");
					}
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (!tetActive)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num2, num3 + 10f * num, 10f * num, 30f), "Game Over!", val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num2, num3 + 20f * num + 5f, 10f * num, 20f), "<size=11>Arrows/WASD: Move, Up/W: Rotate, Down/S: Drop</size>");
	}

	private void TetNewGame()
	{
		tetGrid = new int[20, 10];
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				tetGrid[i, j] = -1;
			}
		}
		tetScore = 0;
		tetActive = true;
		tetTimer = 0f;
		tetSpeed = 0.5f;
		tetPiece = Random.Range(0, 7);
		tetNext = Random.Range(0, 7);
		tetX = 3;
		tetY = 0;
		tetRot = 0;
	}

	private bool TetCanMove(int tx, int ty, int piece, int rot)
	{
		int[,] array = tetShapes[piece];
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				if (array[i, j] != 0)
				{
					int num = tx + j;
					int num2 = ty + i;
					if (num < 0 || num >= 10 || num2 >= 20)
					{
						return false;
					}
					if (num2 >= 0 && tetGrid[num2, num] >= 0)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private void TetPlace()
	{
		int[,] array = tetShapes[tetPiece];
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				if (array[i, j] == 1 && tetY + i >= 0)
				{
					tetGrid[tetY + i, tetX + j] = tetPiece;
				}
			}
		}
		int num = 0;
		for (int num2 = 19; num2 >= 0; num2--)
		{
			bool flag = true;
			for (int k = 0; k < 10; k++)
			{
				if (tetGrid[num2, k] < 0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				num++;
				for (int num3 = num2; num3 > 0; num3--)
				{
					for (int l = 0; l < 10; l++)
					{
						tetGrid[num3, l] = tetGrid[num3 - 1, l];
					}
				}
				for (int m = 0; m < 10; m++)
				{
					tetGrid[0, m] = -1;
				}
				num2++;
			}
		}
		if (num > 0)
		{
			tetScore += num * num * 100;
		}
		tetPiece = tetNext;
		tetNext = Random.Range(0, 7);
		tetX = 3;
		tetY = 0;
		tetRot = 0;
		if (!TetCanMove(tetX, tetY, tetPiece, tetRot))
		{
			tetActive = false;
		}
	}

	private void DrawSolitaire()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Expected O, but got Unknown
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Expected O, but got Unknown
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Expected O, but got Unknown
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Expected O, but got Unknown
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0946: Unknown result type (might be due to invalid IL or missing references)
		//IL_0950: Expected O, but got Unknown
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Expected O, but got Unknown
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		if (solColumns == null)
		{
			SolNewGame();
		}
		bool flag = true;
		List<int>[] array = solFoundation;
		foreach (List<int> list in array)
		{
			if (list.Count < 13)
			{
				flag = false;
			}
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Solitaire</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), (flag && solFoundation[0].Count > 0) ? "<size=12>You Win!</size>" : $"<size=12>Stock: {solStock.Count}  |  Foundation cards: {SolFoundationTotal()}</size>");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			SolNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 52f;
		float num2 = 72f;
		float num3 = 175f;
		float num4 = 72f;
		float num5 = 56f;
		string[] array2 = new string[4] { "♥", "♦", "♣", "♠" };
		Color[] array3 = (Color[])(object)new Color[4]
		{
			Color.red,
			Color.red,
			Color.black,
			Color.black
		};
		string[] array4 = new string[13]
		{
			"A", "2", "3", "4", "5", "6", "7", "8", "9", "10",
			"J", "Q", "K"
		};
		for (int j = 0; j < 4; j++)
		{
			float num6 = num3 + (float)(j + 3) * num5;
			GUI.backgroundColor = new Color(0.15f, 0.45f, 0.15f);
			GUI.Box(new Rect(num6, num4, num, num2), "");
			if (solFoundation[j].Count > 0)
			{
				int num7 = solFoundation[j][solFoundation[j].Count - 1];
				int num8 = num7 / 13;
				int num9 = num7 % 13;
				GUI.backgroundColor = Color.white;
				GUI.color = array3[num8];
				GUIStyle val = new GUIStyle(GUI.skin.label)
				{
					fontSize = 12,
					alignment = (TextAnchor)1,
					fontStyle = (FontStyle)1
				};
				GUI.Label(new Rect(num6 + 2f, num4 + 2f, num - 4f, 18f), array4[num9] + array2[num8], val);
				GUIStyle val2 = new GUIStyle(GUI.skin.label)
				{
					fontSize = 18,
					alignment = (TextAnchor)4
				};
				GUI.Label(new Rect(num6, num4 + 18f, num, 30f), array2[num8], val2);
				GUI.color = Color.white;
			}
		}
		GUI.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
		if (solStock.Count > 0)
		{
			if (GUI.Button(new Rect(num3, num4, num, num2), "\ud83c\udca0"))
			{
				int num10 = Mathf.Min(3, solStock.Count);
				for (int k = 0; k < num10; k++)
				{
					solWastePile.Add(solStock[solStock.Count - 1]);
					solStock.RemoveAt(solStock.Count - 1);
				}
				solWasteActive = solWastePile.Count > 0;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		else if (GUI.Button(new Rect(num3, num4, num, num2), "↻"))
		{
			solWastePile.Reverse();
			solStock.AddRange(solWastePile);
			solWastePile.Clear();
			solWasteActive = false;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (solWastePile.Count > 0)
		{
			int num11 = solWastePile[solWastePile.Count - 1];
			int num12 = num11 / 13;
			int num13 = num11 % 13;
			GUI.backgroundColor = (Color)((solSelectedCol == -2) ? new Color(0.6f, 0.8f, 1f) : Color.white);
			if (GUI.Button(new Rect(num3 + num5, num4, num, num2), ""))
			{
				if (solSelectedCol == -2)
				{
					solSelectedCol = -1;
					solSelectedIdx = -1;
				}
				else
				{
					solSelectedCol = -2;
					solSelectedIdx = solWastePile.Count - 1;
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.color = array3[num12];
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				alignment = (TextAnchor)1,
				fontStyle = (FontStyle)1
			};
			GUI.Label(new Rect(num3 + num5 + 2f, num4 + 2f, num - 4f, 18f), array4[num13] + array2[num12], val3);
			GUIStyle val4 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num3 + num5, num4 + 18f, num, 30f), array2[num12], val4);
			GUI.color = Color.white;
		}
		for (int l = 0; l < 7; l++)
		{
			float num14 = num3 + (float)l * num5;
			float num15 = num4 + num2 + 10f;
			int count = solColumns[l].Count;
			for (int m = 0; m < count; m++)
			{
				int num16 = solColumns[l][m];
				bool flag2 = solColFaceUp[l][m];
				float num17 = num15 + (float)m * 18f;
				if (solSelectedCol == l && solSelectedIdx == m)
				{
					GUI.backgroundColor = new Color(0.6f, 0.8f, 1f);
				}
				else if (flag2)
				{
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUI.backgroundColor = new Color(0.2f, 0.3f, 0.6f);
				}
				if (GUI.Button(new Rect(num14, num17, num, num2 - 10f), "") && flag2)
				{
					if (solSelectedCol >= 0 && solSelectedCol != l)
					{
						if (SolCanPlaceOnColumn(l, solSelectedCol, solSelectedIdx))
						{
							SolMoveCards(l, solSelectedCol, solSelectedIdx);
							SoundManager.Play(SoundManager.DefaultSounds["Button"]);
						}
						else
						{
							solSelectedCol = l;
							solSelectedIdx = m;
						}
					}
					else if (solSelectedCol == -2)
					{
						int num18 = solWastePile[solWastePile.Count - 1];
						if (SolCanPlaceOnColumnFromCard(l, num18))
						{
							solColumns[l].Add(num18);
							solWastePile.RemoveAt(solWastePile.Count - 1);
							solWasteActive = solWastePile.Count > 0;
							SoundManager.Play(SoundManager.DefaultSounds["Button"]);
						}
						solSelectedCol = -1;
						solSelectedIdx = -1;
					}
					else
					{
						solSelectedCol = l;
						solSelectedIdx = m;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
				}
				if (flag2)
				{
					int num19 = num16 / 13;
					int num20 = num16 % 13;
					GUI.color = array3[num19];
					GUIStyle val5 = new GUIStyle(GUI.skin.label)
					{
						fontSize = 10,
						alignment = (TextAnchor)1,
						fontStyle = (FontStyle)1
					};
					GUI.Label(new Rect(num14 + 2f, num17 + 2f, num - 4f, 14f), array4[num20] + array2[num19], val5);
					GUIStyle val6 = new GUIStyle(GUI.skin.label)
					{
						fontSize = 14,
						alignment = (TextAnchor)4
					};
					GUI.Label(new Rect(num14, num17 + 14f, num, 24f), array2[num19], val6);
					GUI.color = Color.white;
				}
			}
			if (count != 0)
			{
				continue;
			}
			GUI.backgroundColor = new Color(0.35f, 0.35f, 0.4f);
			if (!GUI.Button(new Rect(num14, num15, num, num2 - 10f), ""))
			{
				continue;
			}
			if (solSelectedCol == -2)
			{
				int num21 = solWastePile[solWastePile.Count - 1];
				int num22 = num21 / 13;
				if (num21 % 13 == 0)
				{
					solColumns[l].Add(num21);
					solWastePile.RemoveAt(solWastePile.Count - 1);
					solWasteActive = solWastePile.Count > 0;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				solSelectedCol = -1;
				solSelectedIdx = -1;
			}
			else if (solSelectedCol >= 0)
			{
				int num23 = solColumns[solSelectedCol][solSelectedIdx];
				int num24 = num23 / 13;
				int num25 = num23 % 13;
				if (num25 == 12)
				{
					SolMoveCards(l, solSelectedCol, solSelectedIdx);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
		}
		for (int n = 0; n < 4; n++)
		{
			float num26 = num3 + (float)(n + 3) * num5;
			float num27 = num4;
			if (!GUI.Button(new Rect(num26, num27, num, num2), ""))
			{
				continue;
			}
			if (solSelectedCol == -2)
			{
				int num28 = solWastePile[solWastePile.Count - 1];
				if (SolCanPlaceOnFoundation(n, num28))
				{
					solFoundation[n].Add(num28);
					solWastePile.RemoveAt(solWastePile.Count - 1);
					solWasteActive = solWastePile.Count > 0;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				solSelectedCol = -1;
				solSelectedIdx = -1;
			}
			else
			{
				if (solSelectedCol < 0 || solColumns[solSelectedCol].Count <= 0)
				{
					continue;
				}
				int num29 = solColumns[solSelectedCol][(solSelectedCol == solSelectedCol) ? solSelectedIdx : (solColumns[solSelectedCol].Count - 1)];
				if (solSelectedIdx == solColumns[solSelectedCol].Count - 1 && SolCanPlaceOnFoundation(n, num29))
				{
					solFoundation[n].Add(num29);
					solColumns[solSelectedCol].RemoveAt((solSelectedCol == solSelectedCol) ? solSelectedIdx : (solColumns[solSelectedCol].Count - 1));
					if (solColumns[solSelectedCol].Count > 0)
					{
						solColFaceUp[solSelectedCol][solColumns[solSelectedCol].Count - 1] = true;
					}
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				solSelectedCol = -1;
				solSelectedIdx = -1;
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num3, num4 + num2 + 10f + 126f + num2 + 5f, 500f, 20f), "<size=11>Click stock to draw. Click card to select, then click column/foundation to place. Click foundation to auto-move.</size>");
	}

	private bool SolCanPlaceOnFoundation(int f, int card)
	{
		int num = card / 13;
		int num2 = card % 13;
		if (solFoundation[f].Count == 0)
		{
			return num2 == 0;
		}
		int num3 = solFoundation[f][solFoundation[f].Count - 1];
		return num3 / 13 == num && num3 % 13 == num2 - 1;
	}

	private bool SolCanPlaceOnColumn(int targetCol, int fromCol, int fromIdx)
	{
		int num = solColumns[fromCol][fromIdx];
		if (solColumns[targetCol].Count == 0)
		{
			int num2 = num / 13;
			int num3 = num % 13;
			return num3 == 12;
		}
		int num4 = solColumns[targetCol][solColumns[targetCol].Count - 1];
		int num5 = num4 / 13;
		int num6 = num4 % 13;
		int num7 = num / 13;
		int num8 = num % 13;
		return (num5 + num7) % 2 == 1 && num6 == num8 + 1;
	}

	private bool SolCanPlaceOnColumnFromCard(int targetCol, int card)
	{
		if (solColumns[targetCol].Count == 0)
		{
			int num = card / 13;
			int num2 = card % 13;
			return num2 == 12;
		}
		int num3 = solColumns[targetCol][solColumns[targetCol].Count - 1];
		int num4 = num3 / 13;
		int num5 = num3 % 13;
		int num6 = card / 13;
		int num7 = card % 13;
		return (num4 + num6) % 2 == 1 && num5 == num7 + 1;
	}

	private void SolMoveCards(int targetCol, int fromCol, int fromIdx)
	{
		int count = solColumns[fromCol].Count - fromIdx;
		solColumns[targetCol].AddRange(solColumns[fromCol].GetRange(fromIdx, count));
		solColumns[fromCol].RemoveRange(fromIdx, count);
		if (solColumns[fromCol].Count > 0)
		{
			solColFaceUp[fromCol][solColumns[fromCol].Count - 1] = true;
		}
		solSelectedCol = -1;
		solSelectedIdx = -1;
	}

	private int SolFoundationTotal()
	{
		int num = 0;
		List<int>[] array = solFoundation;
		foreach (List<int> list in array)
		{
			num += list.Count;
		}
		return num;
	}

	private void SolNewGame()
	{
		solColumns = new List<int>[7];
		solColFaceUp = new List<bool>[7];
		for (int i = 0; i < 7; i++)
		{
			solColumns[i] = new List<int>();
			solColFaceUp[i] = new List<bool>();
		}
		solFoundation = new List<int>[4];
		for (int j = 0; j < 4; j++)
		{
			solFoundation[j] = new List<int>();
		}
		solStock = new List<int>();
		solWastePile = new List<int>();
		solWasteActive = false;
		solSelectedCol = -1;
		solSelectedIdx = -1;
		List<int> list = new List<int>();
		for (int k = 0; k < 52; k++)
		{
			list.Add(k);
		}
		for (int num = list.Count - 1; num > 0; num--)
		{
			int index = Random.Range(0, num + 1);
			int value = list[num];
			list[num] = list[index];
			list[index] = value;
		}
		int l = 0;
		for (int m = 0; m < 7; m++)
		{
			for (int n = 0; n <= m; n++)
			{
				solColumns[m].Add(list[l]);
				solColFaceUp[m].Add(n == m);
				l++;
			}
		}
		for (; l < 52; l++)
		{
			solStock.Add(list[l]);
		}
	}

	private void DrawChess()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Expected O, but got Unknown
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		if (chBoard == null)
		{
			ChNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Chess</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), (!chGameOver) ? ((chTurn == 1) ? "Your turn (White)" : "AI thinking...") : ((chWinner == 1) ? "You Win!" : "AI Wins!"));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			ChNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 40f;
		float num2 = 180f;
		float num3 = 72f;
		string[] array = new string[13]
		{
			"", "♙", "♖", "♘", "♗", "♕", "♔", "♟", "♜", "♞",
			"♝", "♛", "♚"
		};
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				bool flag = (i + j) % 2 == 0;
				GUI.backgroundColor = ((i == chSelR && j == chSelC) ? new Color(0.4f, 0.8f, 0.4f) : (flag ? new Color(0.45f, 0.35f, 0.25f) : new Color(0.9f, 0.85f, 0.7f)));
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), ""))
				{
					if (chGameOver || chTurn != 1)
					{
						continue;
					}
					int num6 = chBoard[i, j];
					if (chSelR < 0)
					{
						if (num6 > 0 && num6 <= 6)
						{
							chSelR = i;
							chSelC = j;
						}
					}
					else if (num6 > 0 && num6 <= 6)
					{
						chSelR = i;
						chSelC = j;
					}
					else
					{
						ChTryMove(i, j);
					}
				}
				int num7 = chBoard[i, j];
				if (num7 > 0)
				{
					GUI.color = ((num7 <= 6) ? Color.white : Color.black);
					GUIStyle val = new GUIStyle(GUI.skin.label)
					{
						fontSize = 28,
						alignment = (TextAnchor)4
					};
					GUI.Label(new Rect(num4, num5 + 2f, num, num), array[num7], val);
					GUI.color = Color.white;
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (!chGameOver && chTurn == 2)
		{
			ChAIMove();
			chTurn = 1;
			ChCheckWin();
		}
	}

	private void ChNewGame()
	{
		chBoard = new int[8, 8];
		int[] array = new int[8] { 2, 3, 4, 5, 6, 4, 3, 2 };
		for (int i = 0; i < 8; i++)
		{
			chBoard[0, i] = array[i];
			chBoard[1, i] = 1;
			chBoard[6, i] = 7;
			chBoard[7, i] = array[i] + 6;
		}
		chTurn = 1;
		chSelR = (chSelC = -1);
		chGameOver = false;
		chWinner = 0;
	}

	private void ChTryMove(int tr, int tc)
	{
		int num = chSelR;
		int num2 = chSelC;
		int num3 = chBoard[num, num2];
		int num4 = chBoard[tr, tc];
		if (num4 > 0 && num4 <= 6)
		{
			chSelR = num;
			chSelC = num2;
			return;
		}
		int num5 = tr - num;
		int num6 = tc - num2;
		bool flag = false;
		int num7 = num3;
		if (num7 == 1)
		{
			if (num6 == 0 && num5 == -1 && num4 == 0)
			{
				flag = true;
			}
			if (num6 == 0 && num5 == -2 && num == 6 && chBoard[num - 1, num2] == 0 && num4 == 0)
			{
				flag = true;
			}
			if (Math.Abs(num6) == 1 && num5 == -1 && num4 > 6)
			{
				flag = true;
			}
		}
		else if (num7 == 2 || num7 == 8)
		{
			if (num5 == 0 || num6 == 0)
			{
				flag = ChPathClear(num, num2, tr, tc);
			}
		}
		else if (num7 == 3 || num7 == 9)
		{
			if ((Math.Abs(num5) == 2 && Math.Abs(num6) == 1) || (Math.Abs(num5) == 1 && Math.Abs(num6) == 2))
			{
				if (num4 != 0)
				{
					bool num8;
					if (num7 != 3)
					{
						if (num4 <= 0)
						{
							goto IL_0248;
						}
						num8 = num4 <= 6;
					}
					else
					{
						num8 = num4 > 6;
					}
					if (!num8)
					{
						goto IL_0248;
					}
				}
				flag = true;
			}
		}
		else if (num7 == 4 || num7 == 10)
		{
			if (Math.Abs(num5) == Math.Abs(num6))
			{
				flag = ChPathClear(num, num2, tr, tc);
			}
		}
		else if (num7 == 5 || num7 == 11)
		{
			if (num5 == 0 || num6 == 0 || Math.Abs(num5) == Math.Abs(num6))
			{
				flag = ChPathClear(num, num2, tr, tc);
			}
		}
		else if ((num7 == 6 || num7 == 12) && Math.Abs(num5) <= 1 && Math.Abs(num6) <= 1)
		{
			flag = true;
		}
		goto IL_0248;
		IL_0248:
		if (flag)
		{
			chBoard[tr, tc] = num3;
			chBoard[num, num2] = 0;
			if (num7 == 1 && tr == 0)
			{
				chBoard[tr, tc] = 5;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			chSelR = (chSelC = -1);
			chTurn = 2;
		}
	}

	private bool ChPathClear(int sr, int sc, int tr, int tc)
	{
		int num = Math.Sign(tr - sr);
		int num2 = Math.Sign(tc - sc);
		int num3 = sr + num;
		for (int i = sc + num2; num3 != tr || i != tc; i += num2)
		{
			if (chBoard[num3, i] != 0)
			{
				return false;
			}
			num3 += num;
		}
		return true;
	}

	private void ChAIMove()
	{
		List<int[]> list = new List<int[]>();
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				int num = chBoard[i, j];
				if (num < 7)
				{
					continue;
				}
				for (int k = 0; k < 8; k++)
				{
					for (int l = 0; l < 8; l++)
					{
						if (k == i && l == j)
						{
							continue;
						}
						int num2 = chBoard[k, l];
						if (num2 > 0 && num2 <= 6)
						{
							continue;
						}
						int num3 = k - i;
						int num4 = l - j;
						bool flag = false;
						if (num == 7)
						{
							if (num4 == 0 && num3 == 1 && num2 == 0)
							{
								flag = true;
							}
							if (Math.Abs(num4) == 1 && num3 == 1 && num2 > 0 && num2 <= 6)
							{
								flag = true;
							}
						}
						else if (num == 8 || num == 14)
						{
							if (num3 == 0 || num4 == 0)
							{
								flag = ChPathClear(i, j, k, l);
							}
						}
						else if (num == 9 || num == 15)
						{
							if (((Math.Abs(num3) == 2 && Math.Abs(num4) == 1) || (Math.Abs(num3) == 1 && Math.Abs(num4) == 2)) && (num2 == 0 || num2 <= 6))
							{
								flag = true;
							}
						}
						else if (num == 10 || num == 16)
						{
							if (Math.Abs(num3) == Math.Abs(num4))
							{
								flag = ChPathClear(i, j, k, l);
							}
						}
						else if (num == 11 || num == 17)
						{
							if (num3 == 0 || num4 == 0 || Math.Abs(num3) == Math.Abs(num4))
							{
								flag = ChPathClear(i, j, k, l);
							}
						}
						else if ((num == 12 || num == 18) && Math.Abs(num3) <= 1 && Math.Abs(num4) <= 1)
						{
							flag = true;
						}
						if (flag)
						{
							list.Add(new int[4] { i, j, k, l });
						}
					}
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		int[] array = list[0];
		int num5 = -1;
		foreach (int[] item in list)
		{
			int num6 = chBoard[item[2], item[3]];
			int num7 = ((num6 > 0 && num6 <= 6) ? 10 : 0);
			if (num7 > num5)
			{
				num5 = num7;
				array = item;
			}
		}
		chBoard[array[2], array[3]] = chBoard[array[0], array[1]];
		chBoard[array[0], array[1]] = 0;
		int num8 = chBoard[array[2], array[3]];
		if (num8 == 7 && array[2] == 7)
		{
			chBoard[array[2], array[3]] = 11;
		}
		SoundManager.Play(SoundManager.DefaultSounds["Button"]);
	}

	private void ChCheckWin()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (chBoard[i, j] == 6)
				{
					flag = true;
				}
				if (chBoard[i, j] == 12)
				{
					flag2 = true;
				}
			}
		}
		if (!flag)
		{
			chGameOver = true;
			chWinner = 2;
		}
		if (!flag2)
		{
			chGameOver = true;
			chWinner = 1;
		}
	}

	private void DrawWhackAMole()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Expected O, but got Unknown
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		if (!wamActive && wamScore == 0)
		{
			WAMNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Whack-a-Mole</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {wamScore}  Lives: {wamLives}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			WAMNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		wamTimer += Time.deltaTime;
		wamSpawnTimer += Time.deltaTime;
		if (wamActive)
		{
			if (wamSpawnTimer >= 0.8f)
			{
				wamSpawnTimer = 0f;
				int num = Random.Range(0, 3);
				int num2 = Random.Range(0, 3);
				wamGrid[num, num2] = 1;
				float num3 = Mathf.Max(0.4f, 1.2f - (float)wamScore * 0.03f);
				wamMoleTimer = num3;
			}
			wamMoleTimer -= Time.deltaTime;
			if (wamMoleTimer <= 0f)
			{
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if (wamGrid[i, j] == 1)
						{
							wamGrid[i, j] = 0;
						}
					}
				}
				wamLives--;
				if (wamLives <= 0)
				{
					wamActive = false;
				}
			}
		}
		float num4 = 70f;
		float num5 = 200f;
		float num6 = 80f;
		for (int k = 0; k < 3; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				float num7 = num5 + (float)l * (num4 + 8f);
				float num8 = num6 + (float)k * (num4 + 8f);
				if (wamGrid[k, l] == 1)
				{
					GUI.backgroundColor = new Color(0.6f, 0.4f, 0.2f);
					if (GUI.Button(new Rect(num7, num8, num4, num4), "<size=28>\ud83d\udc39</size>"))
					{
						wamGrid[k, l] = 0;
						wamScore += 10;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
				}
				else
				{
					GUI.backgroundColor = new Color(0.35f, 0.55f, 0.25f);
					GUI.Box(new Rect(num7, num8, num4, num4), "");
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (!wamActive && wamScore > 0)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num5, num6 + 3f * (num4 + 8f) + 10f, 3f * num4 + 16f, 30f), "Game Over!", val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num5, num6 + 3f * (num4 + 8f) + 45f, 3f * num4 + 16f, 20f), "<size=11>Click moles as they appear!</size>");
	}

	private void WAMNewGame()
	{
		wamGrid = new int[3, 3];
		wamScore = 0;
		wamLives = 5;
		wamActive = true;
		wamTimer = 0f;
		wamSpawnTimer = 0f;
		wamMoleTimer = 1f;
	}

	private void DrawReactionTest()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Reaction Test</size>");
		if (rtBest > 0f)
		{
			GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Best: {rtBest * 1000f:F0}ms");
		}
		float num = 200f;
		float num2 = 80f;
		float num3 = 400f;
		float num4 = 250f;
		GUI.backgroundColor = guiColorB;
		Event current = Event.current;
		if (rtState == 0)
		{
			GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
			if (GUI.Button(new Rect(num, num2, num3, num4), "<size=22>Click to Start</size>"))
			{
				rtState = 1;
				rtWaitTime = Random.Range(1f, 4f);
				rtTimer = 0f;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		else if (rtState == 1)
		{
			GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
			GUI.Box(new Rect(num, num2, num3, num4), "<size=22>Wait for green...</size>");
			rtTimer += Time.deltaTime;
			if (rtTimer >= rtWaitTime)
			{
				rtState = 2;
				rtTimer = 0f;
			}
		}
		else if (rtState == 2)
		{
			GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
			if (GUI.Button(new Rect(num, num2, num3, num4), "<size=22>CLICK NOW!</size>"))
			{
				float num5 = rtTimer;
				if (rtBest <= 0f || num5 < rtBest)
				{
					rtBest = num5;
				}
				rtState = 0;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			rtTimer += Time.deltaTime;
		}
		else if (rtState == 3)
		{
			GUI.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
			GUI.Box(new Rect(num, num2, num3, num4), "<size=22>Too early! Click to retry</size>");
			if (GUI.Button(new Rect(num, num2, num3, num4), ""))
			{
				rtState = 0;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num, num2 + num4 + 5f, num3, 20f), "<size=11>Click when the screen turns green!</size>");
	}

	private void DrawTypingSpeed()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		if (!tstActive && tstTotal == 0)
		{
			TSNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Typing Speed</size>");
		GUI.Label(new Rect(170f, 48f, 400f, 20f), $"WPM: {tstWPM}  Accuracy: {((tstTotal > 0) ? (tstCorrect * 100 / tstTotal) : 100)}%  Time: {tstTimer:F1}s");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			TSNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 200f;
		float num2 = 80f;
		float num3 = 400f;
		if (tstActive)
		{
			tstTimer += Time.deltaTime;
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 24,
				alignment = (TextAnchor)4,
				wordWrap = true
			};
			GUI.Label(new Rect(num, num2, num3, 60f), tstCurrentWord, val);
			GUI.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
			GUI.SetNextControlName("TypingInput");
			tstTyped = GUI.TextField(new Rect(num, num2 + 70f, num3, 30f), tstTyped, 50);
			if (tstTyped.Length >= tstCurrentWord.Length)
			{
				if (tstTyped == tstCurrentWord)
				{
					tstCorrect++;
				}
				tstTotal++;
				int num4 = Random.Range(0, tstWords.Length);
				tstCurrentWord = tstWords[num4];
				tstTyped = "";
				tstCorrect = ((tstCorrect >= 0) ? tstCorrect : 0);
			}
			if (tstTimer >= 60f)
			{
				tstWPM = tstCorrect;
				tstActive = false;
			}
		}
		else if (tstTotal > 0)
		{
			tstWPM = (int)((float)tstCorrect / Mathf.Max(1f, tstTimer / 60f));
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 20,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + 40f, num3, 40f), $"Final WPM: {tstWPM}", val2);
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num, num2 + 120f, num3, 20f), "<size=11>Type the word and press Enter. 60 second test.</size>");
	}

	private void TSNewGame()
	{
		tstWords = new string[30]
		{
			"the", "quick", "brown", "fox", "jumps", "over", "lazy", "dog", "gorilla", "tag",
			"monkey", "tree", "lobby", "room", "player", "bananas", "swing", "throw", "slide", "climb",
			"branch", "vine", "bounce", "grab", "hang", "kick", "punch", "wave", "dance", "run"
		};
		int num = Random.Range(0, tstWords.Length);
		tstCurrentWord = tstWords[num];
		tstTyped = "";
		tstCorrect = 0;
		tstTotal = 0;
		tstTimer = 0f;
		tstWPM = 0;
		tstActive = true;
		tstStartTime = Time.time;
	}

	private void DrawCatchObjects()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Invalid comparison between Unknown and I4
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Invalid comparison between Unknown and I4
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Invalid comparison between Unknown and I4
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Invalid comparison between Unknown and I4
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Invalid comparison between Unknown and I4
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Expected O, but got Unknown
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Invalid comparison between Unknown and I4
		if (!coActive && coScore == 0)
		{
			CONewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Catch Objects</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {coScore}  Lives: {coLives}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			CONewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (coActive)
		{
			coSpawnTimer += Time.deltaTime;
			coSpeed = 80f + (float)coScore * 3f;
			float num = Mathf.Max(0.4f, 1.5f - (float)coScore * 0.02f);
			if (coSpawnTimer >= num)
			{
				coSpawnTimer = 0f;
				coFalling.Add(new Vector3(Random.Range(10f, coFieldW - 10f), -10f, 0f));
			}
			Event current = Event.current;
			if ((int)current.type == 0)
			{
				coBasketX = Mathf.Clamp(current.mousePosition.x - 200f, 0f, coFieldW - 40f);
			}
			if ((int)current.type == 4)
			{
				if ((int)current.keyCode == 276 || (int)current.keyCode == 97)
				{
					coBasketX = Mathf.Max(0f, coBasketX - 20f);
				}
				if ((int)current.keyCode == 275 || (int)current.keyCode == 100)
				{
					coBasketX = Mathf.Min(coFieldW - 40f, coBasketX + 20f);
				}
			}
			for (int num2 = coFalling.Count - 1; num2 >= 0; num2--)
			{
				Vector3 val = coFalling[num2];
				val.y += coSpeed * Time.deltaTime;
				coFalling[num2] = val;
				if (val.y >= coFieldH - 40f)
				{
					if (Mathf.Abs(val.x - (coBasketX + 20f)) < 30f)
					{
						coScore += 10;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					else
					{
						coLives--;
						if (coLives <= 0)
						{
							coActive = false;
						}
					}
					coFalling.RemoveAt(num2);
				}
			}
		}
		GUI.backgroundColor = new Color(0.35f, 0.55f, 0.8f);
		GUI.Box(new Rect(200f, 80f + coFieldH - 35f, coFieldW, 35f), "");
		GUI.backgroundColor = new Color(0.8f, 0.6f, 0.2f);
		GUI.Box(new Rect(200f + coBasketX, 80f + coFieldH - 50f, 40f, 50f), "\ud83e\uddfa");
		foreach (Vector3 item in coFalling)
		{
			GUI.backgroundColor = Color.yellow;
			GUI.Box(new Rect(200f + item.x - 8f, 80f + item.y - 8f, 16f, 16f), "\ud83c\udf4c");
		}
		GUI.backgroundColor = guiColorB;
		if (!coActive && coScore > 0)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(200f, 80f + coFieldH / 2f - 15f, coFieldW, 30f), "Game Over!", val2);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(200f, 80f + coFieldH + 5f, coFieldW, 20f), "<size=11>Mouse or A/D to catch falling bananas!</size>");
	}

	private void CONewGame()
	{
		coBasketX = coFieldW / 2f - 20f;
		coFalling = new List<Vector3>();
		coScore = 0;
		coLives = 5;
		coActive = true;
		coSpawnTimer = 0f;
		coSpeed = 80f;
	}

	private void DrawPacman()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Invalid comparison between Unknown and I4
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Invalid comparison between Unknown and I4
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Invalid comparison between Unknown and I4
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Invalid comparison between Unknown and I4
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Invalid comparison between Unknown and I4
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Invalid comparison between Unknown and I4
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Invalid comparison between Unknown and I4
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Invalid comparison between Unknown and I4
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Expected O, but got Unknown
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Invalid comparison between Unknown and I4
		if (pacMaze == null)
		{
			PACNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Pac-Man</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {pacScore}  Lives: {pacLives}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			PACNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		Event current = Event.current;
		if ((int)current.type == 4 && pacActive)
		{
			if (((int)current.keyCode == 273 || (int)current.keyCode == 119) && PACCanMove(pacPR - 1, pacPC))
			{
				pacDir = 0;
			}
			if (((int)current.keyCode == 274 || (int)current.keyCode == 115) && PACCanMove(pacPR + 1, pacPC))
			{
				pacDir = 1;
			}
			if (((int)current.keyCode == 276 || (int)current.keyCode == 97) && PACCanMove(pacPR, pacPC - 1))
			{
				pacDir = 2;
			}
			if (((int)current.keyCode == 275 || (int)current.keyCode == 100) && PACCanMove(pacPR, pacPC + 1))
			{
				pacDir = 3;
			}
			current.Use();
		}
		if (pacActive)
		{
			pacMoveTimer += Time.deltaTime;
			float num = 0.15f;
			if (pacMoveTimer >= num)
			{
				pacMoveTimer = 0f;
				int num2 = ((pacDir == 0) ? (-1) : ((pacDir == 1) ? 1 : 0));
				int num3 = ((pacDir == 2) ? (-1) : ((pacDir == 3) ? 1 : 0));
				if (PACCanMove(pacPR + num2, pacPC + num3))
				{
					pacPR += num2;
					pacPC += num3;
					if (pacMaze[pacPR, pacPC] == 1)
					{
						pacMaze[pacPR, pacPC] = 0;
						pacScore += 10;
					}
				}
				pacGhostTimer += Time.deltaTime;
				if (pacGhostTimer >= 0.3f)
				{
					pacGhostTimer = 0f;
					for (int i = 0; i < pacGhosts.Count; i++)
					{
						int[] array = pacGhosts[i];
						int num4 = pacGhostDirs[i];
						int[] array2 = new int[4] { -1, 1, -10, 10 };
						int num5 = array2[Random.Range(0, 4)];
						int num6 = array[0] + num5 switch
						{
							10 => 1, 
							-10 => -1, 
							_ => 0, 
						};
						int num7 = array[1] + num5 switch
						{
							1 => 1, 
							-1 => -1, 
							_ => 0, 
						};
						if (PACCanMove(num6, num7) && pacMaze[num6, num7] != 2)
						{
							pacGhosts[i] = new int[2] { num6, num7 };
							pacGhostDirs[i] = num5;
						}
						if (pacGhosts[i][0] == pacPR && pacGhosts[i][1] == pacPC)
						{
							pacLives--;
							pacPR = 1;
							pacPC = 1;
							if (pacLives <= 0)
							{
								pacActive = false;
							}
						}
					}
				}
			}
		}
		float num8 = 24f;
		float num9 = 200f;
		float num10 = 72f;
		for (int j = 0; j < pacMaze.GetLength(0); j++)
		{
			for (int k = 0; k < pacMaze.GetLength(1); k++)
			{
				float num11 = num9 + (float)k * num8;
				float num12 = num10 + (float)j * num8;
				if (pacMaze[j, k] == 2)
				{
					GUI.backgroundColor = new Color(0.2f, 0.3f, 0.8f);
					GUI.Box(new Rect(num11, num12, num8 - 1f, num8 - 1f), "");
				}
				else if (pacMaze[j, k] == 1)
				{
					GUI.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
					GUI.Box(new Rect(num11, num12, num8 - 1f, num8 - 1f), "");
					GUI.backgroundColor = Color.yellow;
					GUI.Box(new Rect(num11 + 8f, num12 + 8f, 8f, 8f), "");
				}
				else
				{
					GUI.backgroundColor = new Color(0.05f, 0.05f, 0.1f);
					GUI.Box(new Rect(num11, num12, num8 - 1f, num8 - 1f), "");
				}
			}
		}
		GUI.backgroundColor = Color.yellow;
		GUI.Box(new Rect(num9 + (float)pacPC * num8 + 2f, num10 + (float)pacPR * num8 + 2f, num8 - 5f, num8 - 5f), "●");
		foreach (int[] pacGhost in pacGhosts)
		{
			GUI.backgroundColor = new Color(0.9f, 0.2f, 0.2f);
			GUI.Box(new Rect(num9 + (float)pacGhost[1] * num8 + 2f, num10 + (float)pacGhost[0] * num8 + 2f, num8 - 5f, num8 - 5f), "\ud83d\udc7b");
		}
		GUI.backgroundColor = guiColorB;
		if (!pacActive && pacScore > 0)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num9, num10 + 12f * num8, (float)pacMaze.GetLength(1) * num8, 30f), "Game Over!", val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num9, num10 + (float)pacMaze.GetLength(0) * num8 + 5f, (float)pacMaze.GetLength(1) * num8, 20f), "<size=11>Arrows/WASD to move. Eat dots, avoid ghosts!</size>");
	}

	private void PACNewGame()
	{
		int num = 12;
		int num2 = 15;
		pacMaze = new int[num, num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if (i == 0 || i == num - 1 || j == 0 || j == num2 - 1)
				{
					pacMaze[i, j] = 2;
				}
				else
				{
					pacMaze[i, j] = 1;
				}
			}
		}
		int[] array = new int[7] { 3, 4, 5, 6, 7, 8, 9 };
		int[] array2 = new int[7] { 7, 7, 3, 11, 5, 9, 7 };
		for (int k = 0; k < array.Length; k++)
		{
			pacMaze[array[k], array2[k]] = 2;
		}
		pacMaze[1, 1] = 0;
		pacPR = 1;
		pacPC = 1;
		pacDir = 3;
		pacScore = 0;
		pacLives = 3;
		pacActive = true;
		pacMoveTimer = 0f;
		pacGhostTimer = 0f;
		pacGhosts = new List<int[]>
		{
			new int[2]
			{
				num - 2,
				num2 - 2
			},
			new int[2]
			{
				1,
				num2 - 2
			}
		};
		pacGhostDirs = new List<int> { 2, 2 };
	}

	private bool PACCanMove(int r, int c)
	{
		if (r < 0 || r >= pacMaze.GetLength(0) || c < 0 || c >= pacMaze.GetLength(1))
		{
			return false;
		}
		return pacMaze[r, c] != 2;
	}

	private void DrawTankBattle()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Invalid comparison between Unknown and I4
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Invalid comparison between Unknown and I4
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Invalid comparison between Unknown and I4
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Invalid comparison between Unknown and I4
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Invalid comparison between Unknown and I4
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Invalid comparison between Unknown and I4
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Expected O, but got Unknown
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		if (!tbActive && tbScore == 0)
		{
			TBNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Tank Battle</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {tbScore}  Lives: {tbLives}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			TBNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 200f;
		float num2 = 72f;
		float num3 = 400f;
		float num4 = 300f;
		if (tbActive)
		{
			Event current = Event.current;
			if ((int)current.type == 0)
			{
				Vector2 mousePosition = current.mousePosition;
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(mousePosition.x - num, mousePosition.y - num2);
				Vector2 val2 = val - new Vector2(tbPX, tbPY);
				Vector2 normalized = ((Vector2)(ref val2)).normalized;
				if (tbShootCooldown <= 0f)
				{
					tbBullets.Add(new Vector3(tbPX, tbPY, 0f));
					tbBulletPlayer.Add(item: true);
					tbShootCooldown = 0.3f;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
			if ((int)current.type == 4)
			{
				if ((int)current.keyCode == 119)
				{
					tbPY = Mathf.Max(10f, tbPY - 15f);
				}
				if ((int)current.keyCode == 115)
				{
					tbPY = Mathf.Min(num4 - 10f, tbPY + 15f);
				}
				if ((int)current.keyCode == 97)
				{
					tbPX = Mathf.Max(10f, tbPX - 15f);
				}
				if ((int)current.keyCode == 100)
				{
					tbPX = Mathf.Min(num3 - 10f, tbPX + 15f);
				}
				current.Use();
			}
			tbShootCooldown -= Time.deltaTime;
			tbSpawnTimer += Time.deltaTime;
			float num5 = Mathf.Max(0.5f, 2f - (float)tbScore * 0.02f);
			if (tbSpawnTimer >= num5)
			{
				tbSpawnTimer = 0f;
				float num6 = Random.Range(10f, num3 - 10f);
				tbEnemies.Add(new Vector4(num6, -10f, 0f, 1f));
				tbEnemyHP.Add(1f);
			}
			for (int num7 = tbEnemies.Count - 1; num7 >= 0; num7--)
			{
				Vector4 val3 = tbEnemies[num7];
				val3.y += 40f * Time.deltaTime;
				tbEnemies[num7] = val3;
				if (val3.y > num4)
				{
					tbEnemies.RemoveAt(num7);
					tbEnemyHP.RemoveAt(num7);
				}
				float num8 = val3.x - tbPX;
				float num9 = val3.y - tbPY;
				if (Mathf.Sqrt(num8 * num8 + num9 * num9) < 15f)
				{
					tbLives--;
					tbEnemies.RemoveAt(num7);
					tbEnemyHP.RemoveAt(num7);
					if (tbLives <= 0)
					{
						tbActive = false;
					}
				}
			}
			for (int num10 = tbBullets.Count - 1; num10 >= 0; num10--)
			{
				Vector3 val4 = tbBullets[num10];
				if (tbBulletPlayer[num10])
				{
					val4.y -= 300f * Time.deltaTime;
					tbBullets[num10] = val4;
					if (val4.y < -10f)
					{
						tbBullets.RemoveAt(num10);
						tbBulletPlayer.RemoveAt(num10);
					}
					else
					{
						for (int num11 = tbEnemies.Count - 1; num11 >= 0; num11--)
						{
							Vector4 val5 = tbEnemies[num11];
							float num12 = val4.x - val5.x;
							float num13 = val4.y - val5.y;
							if (Mathf.Sqrt(num12 * num12 + num13 * num13) < 12f)
							{
								float num14 = tbEnemyHP[num11] - 1f;
								tbEnemyHP[num11] = num14;
								if (num14 <= 0f)
								{
									tbEnemies.RemoveAt(num11);
									tbEnemyHP.RemoveAt(num11);
									tbScore += 25;
									SoundManager.Play(SoundManager.DefaultSounds["Button"]);
								}
								tbBullets.RemoveAt(num10);
								tbBulletPlayer.RemoveAt(num10);
								break;
							}
						}
					}
				}
			}
		}
		GUI.backgroundColor = new Color(0.15f, 0.18f, 0.12f);
		GUI.Box(new Rect(num, num2, num3, num4), "");
		GUI.backgroundColor = Color.green;
		GUI.Box(new Rect(num + tbPX - 8f, num2 + tbPY - 8f, 16f, 16f), "▲");
		GUI.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
		foreach (Vector4 tbEnemy in tbEnemies)
		{
			if (!(tbEnemy.y < 0f) && !(tbEnemy.y > num4))
			{
				GUI.Box(new Rect(num + tbEnemy.x - 8f, num2 + tbEnemy.y - 8f, 16f, 16f), "▼");
			}
		}
		GUI.backgroundColor = Color.yellow;
		foreach (Vector3 tbBullet in tbBullets)
		{
			GUI.Box(new Rect(num + tbBullet.x - 2f, num2 + tbBullet.y - 6f, 4f, 12f), "");
		}
		GUI.backgroundColor = guiColorB;
		if (!tbActive && tbScore > 0)
		{
			GUI.color = new Color(1f, 0.4f, 0.4f);
			GUIStyle val6 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + num4 / 2f - 15f, num3, 30f), "Game Over!", val6);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num, num2 + num4 + 5f, num3, 20f), "<size=11>WASD to move, Click to shoot!</size>");
	}

	private void TBNewGame()
	{
		tbPX = 200f;
		tbPY = 280f;
		tbEnemies = new List<Vector4>();
		tbEnemyHP = new List<float>();
		tbBullets = new List<Vector3>();
		tbBulletPlayer = new List<bool>();
		tbScore = 0;
		tbLives = 5;
		tbActive = true;
		tbSpawnTimer = 0f;
		tbShootCooldown = 0f;
	}

	private void DrawBattleship()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		if (bsPlayerBoard == null)
		{
			BSNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Battleship</size>");
		if (bsPhase == 0)
		{
			GUI.Label(new Rect(170f, 48f, 500f, 20f), "Place ships! Click to place, R to rotate");
		}
		else if (bsPhase == 1)
		{
			GUI.Label(new Rect(170f, 48f, 500f, 20f), "Fire! Click on enemy grid");
		}
		else
		{
			GUI.Label(new Rect(170f, 48f, 500f, 20f), (!bsGameOver) ? "" : ((bsPlayerHits >= 17) ? "You Win!" : "You Lose!"));
		}
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			BSNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		if (bsPhase == 0 && GUI.Button(new Rect(520f, 46f, 70f, 22f), bsPlacingH ? "H→V" : "V→H"))
		{
			bsPlacingH = !bsPlacingH;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 24f;
		string[] array = new string[5] { "2", "3", "3", "4", "5" };
		int[] array2 = new int[5] { 2, 3, 3, 4, 5 };
		float num2 = 180f;
		float num3 = 430f;
		float num4 = 72f;
		GUI.Label(new Rect(num2, num4 - 2f, num * 10f, 18f), "<size=12>Your Grid</size>");
		GUI.Label(new Rect(num3, num4 - 2f, num * 10f, 18f), "<size=12>Enemy Grid</size>");
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				float num5 = num2 + (float)j * num;
				float num6 = num4 + 18f + (float)i * num;
				float num7 = num3 + (float)j * num;
				float num8 = num4 + 18f + (float)i * num;
				bool flag = bsPlayerShips[i, j];
				GUI.backgroundColor = ((bsPlayerBoard[i, j] != 2 && bsPlayerBoard[i, j] != 3) ? (flag ? new Color(0.5f, 0.5f, 0.55f) : new Color(0.3f, 0.4f, 0.5f)) : (flag ? new Color(0.9f, 0.3f, 0.3f) : new Color(0.3f, 0.5f, 0.9f)));
				if (GUI.Button(new Rect(num5, num6, num - 1f, num - 1f), "") && bsPhase == 0 && !flag)
				{
					bsSelR = i;
					bsSelC = j;
					if (BSPlaceShip(i, j, array2[bsPlacingShip], bsPlacingH))
					{
						bsPlacingShip++;
						if (bsPlacingShip >= 5)
						{
							bsPhase = 1;
						}
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
				}
				bool flag2 = bsEnemyBoard[i, j] == 2 || bsEnemyBoard[i, j] == 3;
				bool flag3 = bsEnemyShips[i, j];
				GUI.backgroundColor = ((!flag2) ? new Color(0.3f, 0.5f, 0.6f) : (flag3 ? new Color(0.9f, 0.2f, 0.2f) : new Color(0.3f, 0.5f, 0.9f)));
				if (GUI.Button(new Rect(num7, num8, num - 1f, num - 1f), "") && bsPhase == 1 && !bsGameOver && bsEnemyBoard[i, j] == 0)
				{
					bsEnemyBoard[i, j] = (flag3 ? 2 : 3);
					if (flag3)
					{
						bsPlayerHits++;
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					else
					{
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					}
					int num9 = Random.Range(0, 10);
					int num10 = Random.Range(0, 10);
					while (bsPlayerBoard[num9, num10] != 0)
					{
						num9 = Random.Range(0, 10);
						num10 = Random.Range(0, 10);
					}
					bsPlayerBoard[num9, num10] = (bsPlayerShips[num9, num10] ? 2 : 3);
					if (bsPlayerShips[num9, num10])
					{
						bsEnemyHits++;
					}
					if (bsPlayerHits >= 17 || bsEnemyHits >= 17)
					{
						bsGameOver = true;
					}
				}
			}
		}
		GUI.backgroundColor = guiColorB;
	}

	private void BSNewGame()
	{
		bsPlayerBoard = new int[10, 10];
		bsEnemyBoard = new int[10, 10];
		bsPlayerShips = new bool[10, 10];
		bsEnemyShips = new bool[10, 10];
		bsPhase = 0;
		bsSelR = (bsSelC = -1);
		bsPlacingShip = 0;
		bsPlacingH = true;
		bsPlayerHits = (bsEnemyHits = 0);
		bsGameOver = false;
		int[] array = new int[5] { 2, 3, 3, 4, 5 };
		for (int i = 0; i < 5; i++)
		{
			bool flag = false;
			while (!flag)
			{
				bool flag2 = Random.value > 0.5f;
				int num = Random.Range(0, 10);
				int num2 = Random.Range(0, 10);
				if (BSCanPlace(num, num2, array[i], flag2, bsEnemyShips))
				{
					for (int j = 0; j < array[i]; j++)
					{
						int num3 = (flag2 ? num : (num + j));
						int num4 = (flag2 ? (num2 + j) : num2);
						bsEnemyShips[num3, num4] = true;
					}
					flag = true;
				}
			}
		}
	}

	private bool BSCanPlace(int r, int c, int size, bool hor, bool[,] board)
	{
		for (int i = 0; i < size; i++)
		{
			int num = (hor ? r : (r + i));
			int num2 = (hor ? (c + i) : c);
			if (num >= 10 || num2 >= 10 || board[num, num2])
			{
				return false;
			}
		}
		return true;
	}

	private bool BSPlaceShip(int r, int c, int size, bool hor)
	{
		if (!BSCanPlace(r, c, size, hor, bsPlayerShips))
		{
			return false;
		}
		for (int i = 0; i < size; i++)
		{
			int num = (hor ? r : (r + i));
			int num2 = (hor ? (c + i) : c);
			bsPlayerShips[num, num2] = true;
		}
		return true;
	}

	private void DrawYahtzee()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Expected O, but got Unknown
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		if (yzDice == null)
		{
			YZNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Yahtzee</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Total: {yzTotal}  Rerolls: {yzRerolls}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			YZNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 200f;
		float num2 = 72f;
		string[] array = new string[6] { "⚀", "⚁", "⚂", "⚃", "⚄", "⚅" };
		for (int i = 0; i < 5; i++)
		{
			GUI.backgroundColor = (yzHeld[i] ? new Color(0.3f, 0.7f, 0.3f) : new Color(0.6f, 0.6f, 0.65f));
			if (GUI.Button(new Rect(num + (float)i * 50f, num2, 45f, 50f), (yzDice[i] > 0) ? array[yzDice[i] - 1] : ""))
			{
				yzHeld[i] = !yzHeld[i];
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorA;
		if (yzRerolls > 0 && !yzGameOver)
		{
			if (GUI.Button(new Rect(num, num2 + 58f, 120f, 25f), "Reroll Held"))
			{
				yzRerolls--;
				for (int j = 0; j < 5; j++)
				{
					if (!yzHeld[j])
					{
						yzDice[j] = Random.Range(1, 7);
					}
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			if (GUI.Button(new Rect(num + 130f, num2 + 58f, 120f, 25f), "Roll All"))
			{
				yzRerolls--;
				for (int k = 0; k < 5; k++)
				{
					if (!yzHeld[k])
					{
						yzDice[k] = Random.Range(1, 7);
					}
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		string[] array2 = new string[13]
		{
			"Ones", "Twos", "Threes", "Fours", "Fives", "Sixes", "3 of Kind", "4 of Kind", "Full House", "Sm Straight",
			"Lg Straight", "Yahtzee", "Chance"
		};
		float num3 = num2 + 90f;
		for (int l = 0; l < 13; l++)
		{
			if (yzUsed[l])
			{
				GUI.backgroundColor = new Color(0.35f, 0.35f, 0.4f);
				GUI.Label(new Rect(num, num3 + (float)l * 20f, 120f, 19f), $"{array2[l]}: {yzScores[l]}");
				continue;
			}
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(num, num3 + (float)l * 20f, 120f, 19f), array2[l]))
			{
				yzScores[l] = YZCalcScore(l);
				yzUsed[l] = true;
				yzTotal += yzScores[l];
				for (int m = 0; m < 5; m++)
				{
					yzDice[m] = Random.Range(1, 7);
					yzHeld[m] = false;
				}
				yzRerolls = 2;
				if (Array.TrueForAll(yzUsed, (bool u) => u))
				{
					yzGameOver = true;
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		if (yzGameOver)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = Color.green;
			GUI.Label(new Rect(num + 200f, num2, 200f, 30f), $"Final Score: {yzTotal}", val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num, num3 + 260f + 10f, 300f, 20f), "<size=11>Click dice to hold, reroll, then score a category</size>");
	}

	private int YZCalcScore(int cat)
	{
		int[] array = new int[7];
		int num = 0;
		int[] array2 = yzDice;
		foreach (int num2 in array2)
		{
			array[num2]++;
			num += num2;
		}
		switch (cat)
		{
		case 0:
			return array[1];
		case 1:
			return array[2] * 2;
		case 2:
			return array[3] * 3;
		case 3:
			return array[4] * 4;
		case 4:
			return array[5] * 5;
		case 5:
			return array[6] * 6;
		case 6:
		{
			for (int l = 1; l <= 6; l++)
			{
				if (array[l] >= 3)
				{
					return num;
				}
			}
			return 0;
		}
		case 7:
		{
			for (int k = 1; k <= 6; k++)
			{
				if (array[k] >= 4)
				{
					return num;
				}
			}
			return 0;
		}
		case 8:
		{
			bool flag = false;
			bool flag2 = false;
			for (int j = 1; j <= 6; j++)
			{
				if (array[j] == 3)
				{
					flag = true;
				}
				if (array[j] == 2)
				{
					flag2 = true;
				}
			}
			return (flag && flag2) ? 25 : 0;
		}
		case 9:
		{
			bool flag3 = false;
			if (array[2] >= 1 && array[3] >= 1 && array[4] >= 1 && array[5] >= 1)
			{
				flag3 = true;
			}
			if (array[1] >= 1 && array[2] >= 1 && array[3] >= 1 && array[4] >= 1)
			{
				flag3 = true;
			}
			return flag3 ? 30 : 0;
		}
		case 10:
			if (array[1] >= 1 && array[2] >= 1 && array[3] >= 1 && array[4] >= 1 && array[5] >= 1)
			{
				return 40;
			}
			if (array[2] >= 1 && array[3] >= 1 && array[4] >= 1 && array[5] >= 1 && array[6] >= 1)
			{
				return 40;
			}
			return 0;
		case 11:
			return 50;
		case 12:
			return num;
		default:
			return 0;
		}
	}

	private void YZNewGame()
	{
		yzDice = new int[5];
		yzHeld = new bool[5];
		yzScores = new int[13];
		yzUsed = new bool[13];
		yzRerolls = 2;
		yzTotal = 0;
		yzGameOver = false;
		for (int i = 0; i < 5; i++)
		{
			yzDice[i] = Random.Range(1, 7);
			yzHeld[i] = false;
		}
	}

	private void DrawColorMatch()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Expected O, but got Unknown
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Color Match</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {cmScore}  Time: {cmTimer:F1}s");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			CMNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 200f;
		float num2 = 72f;
		if (cmActive)
		{
			cmTimer -= Time.deltaTime;
			if (cmTimer <= 0f)
			{
				cmActive = false;
			}
			cmSliderR = GUI.HorizontalSlider(new Rect(num, num2 + 30f, 300f, 20f), cmSliderR, 0f, 1f);
			cmSliderG = GUI.HorizontalSlider(new Rect(num, num2 + 60f, 300f, 20f), cmSliderG, 0f, 1f);
			cmSliderB = GUI.HorizontalSlider(new Rect(num, num2 + 90f, 300f, 20f), cmSliderB, 0f, 1f);
			GUI.Label(new Rect(num, num2 + 15f, 300f, 15f), $"<size=12>R: {cmSliderR:F2}</size>");
			GUI.Label(new Rect(num, num2 + 45f, 300f, 15f), $"<size=12>G: {cmSliderG:F2}</size>");
			GUI.Label(new Rect(num, num2 + 75f, 300f, 15f), $"<size=12>B: {cmSliderB:F2}</size>");
			GUI.backgroundColor = cmPlayer;
			GUI.Box(new Rect(num, num2 + 120f, 140f, 80f), "");
			GUI.backgroundColor = cmTarget;
			GUI.Box(new Rect(num + 160f, num2 + 120f, 140f, 80f), "");
			GUI.backgroundColor = guiColorB;
			GUI.Label(new Rect(num, num2 + 210f, 140f, 18f), "<size=11>Your Color</size>");
			GUI.Label(new Rect(num + 160f, num2 + 210f, 140f, 18f), "<size=11>Target Color</size>");
			GUI.backgroundColor = guiColorA;
			cmPlayer = new Color(cmSliderR, cmSliderG, cmSliderB);
			if (GUI.Button(new Rect(num + 100f, num2 + 235f, 100f, 25f), "Submit"))
			{
				float num3 = Mathf.Abs(cmTarget.r - cmPlayer.r) + Mathf.Abs(cmTarget.g - cmPlayer.g) + Mathf.Abs(cmTarget.b - cmPlayer.b);
				int num4 = Mathf.Max(0, (int)((1f - num3 / 3f) * 100f));
				cmScore += num4;
				CMNewTarget();
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		else if (cmScore > 0)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(num, num2 + 100f, 300f, 30f), $"Final Score: {cmScore}", val);
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num, num2 + 270f, 300f, 20f), "<size=11>Match the target color with RGB sliders!</size>");
	}

	private void CMNewGame()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		cmScore = 0;
		cmTimer = 60f;
		cmActive = true;
		cmSliderR = (cmSliderG = (cmSliderB = 0.5f));
		cmPlayer = new Color(0.5f, 0.5f, 0.5f);
		CMNewTarget();
	}

	private void CMNewTarget()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		cmTarget = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
	}

	private void DrawPipePuzzle()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Expected O, but got Unknown
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		if (ppGrid == null)
		{
			PPNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Pipe Puzzle</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), ppSolved ? "Solved!" : "Click pipes to rotate them");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			PPNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 50f;
		float num2 = 200f;
		float num3 = 72f;
		string[] array = new string[17]
		{
			"─", "│", "┌", "┐", "└", "┘", "┬", "┴", "├", "┤",
			"┼", "═", "║", "╔", "╗", "╚", "╝"
		};
		for (int i = 0; i < ppH; i++)
		{
			for (int j = 0; j < ppW; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				GUI.backgroundColor = (ppSolved ? new Color(0.2f, 0.6f, 0.2f) : new Color(0.35f, 0.4f, 0.5f));
				if (GUI.Button(new Rect(num4, num5, num - 2f, num - 2f), "") && !ppSolved)
				{
					ppRotation[i, j] = (ppRotation[i, j] + 1) % 4;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					PPCheck();
				}
				int num6 = ppGrid[i, j];
				GUI.color = Color.white;
				GUIStyle val = new GUIStyle(GUI.skin.label)
				{
					fontSize = 28,
					alignment = (TextAnchor)4
				};
				GUI.Label(new Rect(num4, num5 + 5f, num, num), array[num6 % array.Length], val);
				GUI.color = Color.white;
			}
		}
		GUI.backgroundColor = guiColorB;
	}

	private void PPNewGame()
	{
		ppGrid = new int[ppH, ppW];
		ppRotation = new int[ppH, ppW];
		ppSolved = false;
		for (int i = 0; i < ppH; i++)
		{
			for (int j = 0; j < ppW; j++)
			{
				ppGrid[i, j] = Random.Range(0, 5);
				ppRotation[i, j] = 0;
			}
		}
	}

	private void PPCheck()
	{
		bool flag = true;
		for (int i = 0; i < ppH; i++)
		{
			for (int j = 0; j < ppW; j++)
			{
				if (ppRotation[i, j] != 0)
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			ppSolved = true;
		}
	}

	private void DrawLightsOut()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		if (loGrid == null)
		{
			LONewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Lights Out</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Moves: {loMoves}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			LONewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 55f;
		float num2 = 220f;
		float num3 = 80f;
		bool flag = true;
		for (int i = 0; i < loSize; i++)
		{
			for (int j = 0; j < loSize; j++)
			{
				if (loGrid[i, j])
				{
					flag = false;
				}
			}
		}
		for (int k = 0; k < loSize; k++)
		{
			for (int l = 0; l < loSize; l++)
			{
				float num4 = num2 + (float)l * num;
				float num5 = num3 + (float)k * num;
				GUI.backgroundColor = (loGrid[k, l] ? new Color(1f, 0.9f, 0.3f) : new Color(0.2f, 0.2f, 0.25f));
				if (GUI.Button(new Rect(num4, num5, num - 2f, num - 2f), ""))
				{
					LOToggle(k, l);
					loMoves++;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (flag && loMoves > 0)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = Color.green;
			GUI.Label(new Rect(num2, num3 + (float)loSize * num + 10f, (float)loSize * num, 30f), $"Solved in {loMoves} moves!", val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(num2, num3 + (float)loSize * num + 45f, (float)loSize * num, 20f), "<size=11>Click a light to toggle it and its neighbors</size>");
	}

	private void LONewGame()
	{
		loGrid = new bool[loSize, loSize];
		loMoves = 0;
		for (int i = 0; i < 8; i++)
		{
			LOToggle(Random.Range(0, loSize), Random.Range(0, loSize));
		}
	}

	private void LOToggle(int r, int c)
	{
		loGrid[r, c] = !loGrid[r, c];
		if (r > 0)
		{
			loGrid[r - 1, c] = !loGrid[r - 1, c];
		}
		if (r < loSize - 1)
		{
			loGrid[r + 1, c] = !loGrid[r + 1, c];
		}
		if (c > 0)
		{
			loGrid[r, c - 1] = !loGrid[r, c - 1];
		}
		if (c < loSize - 1)
		{
			loGrid[r, c + 1] = !loGrid[r, c + 1];
		}
	}

	private void DrawNonogram()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Expected O, but got Unknown
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		if (nnpGrid == null)
		{
			NNNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Nonogram</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), nnpSolved ? "Solved!" : "Click to fill, Right-click to mark X");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(430f, 46f, 80f, 22f), "New Game"))
		{
			NNNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 36f;
		float num2 = 250f;
		float num3 = 80f;
		Event current = Event.current;
		for (int i = 0; i < nnpH; i++)
		{
			if (i < nnpRowClues.Length && nnpRowClues[i] != null)
			{
				string text = string.Join(" ", nnpRowClues[i]);
				GUI.Label(new Rect(num2 - 60f, num3 + (float)i * num, 58f, num), "<size=12>" + text + "</size>");
			}
			for (int j = 0; j < nnpW; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				if (i == 0 && j < nnpColClues.Length && nnpColClues[j] != null)
				{
					string text2 = string.Join("\n", nnpColClues[j]);
					GUI.Label(new Rect(num4, num3 - 40f, num, 38f), "<size=11>" + text2 + "</size>");
				}
				if (nnpGrid[i, j] == 1)
				{
					GUI.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
				}
				else if (nnpGrid[i, j] == 2)
				{
					GUI.backgroundColor = new Color(0.4f, 0.4f, 0.45f);
				}
				else
				{
					GUI.backgroundColor = new Color(0.75f, 0.75f, 0.8f);
				}
				if (GUI.Button(new Rect(num4, num5, num - 2f, num - 2f), (nnpGrid[i, j] == 2) ? "×" : "") && !nnpSolved)
				{
					if (current.button == 0)
					{
						nnpGrid[i, j] = ((nnpGrid[i, j] != 1) ? 1 : 0);
					}
					else
					{
						nnpGrid[i, j] = ((nnpGrid[i, j] != 2) ? 2 : 0);
					}
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					NNCheck();
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (nnpSolved)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = Color.green;
			GUI.Label(new Rect(num2, num3 + (float)nnpH * num + 10f, (float)nnpW * num, 30f), "Congratulations!", val);
			GUI.color = Color.white;
		}
	}

	private void NNNewGame()
	{
		nnpGrid = new int[nnpH, nnpW];
		nnpSolution = new int[nnpH, nnpW];
		nnpSolved = false;
		for (int i = 0; i < nnpH; i++)
		{
			for (int j = 0; j < nnpW; j++)
			{
				nnpSolution[i, j] = Random.Range(0, 2);
				nnpGrid[i, j] = 0;
			}
		}
		nnpRowClues = new List<int>[nnpH];
		nnpColClues = new List<int>[nnpW];
		for (int k = 0; k < nnpH; k++)
		{
			nnpRowClues[k] = new List<int>();
			int num = 0;
			for (int l = 0; l < nnpW; l++)
			{
				if (nnpSolution[k, l] == 1)
				{
					num++;
				}
				else if (num > 0)
				{
					nnpRowClues[k].Add(num);
					num = 0;
				}
			}
			if (num > 0)
			{
				nnpRowClues[k].Add(num);
			}
			if (nnpRowClues[k].Count == 0)
			{
				nnpRowClues[k].Add(0);
			}
		}
		for (int m = 0; m < nnpW; m++)
		{
			nnpColClues[m] = new List<int>();
			int num2 = 0;
			for (int n = 0; n < nnpH; n++)
			{
				if (nnpSolution[n, m] == 1)
				{
					num2++;
				}
				else if (num2 > 0)
				{
					nnpColClues[m].Add(num2);
					num2 = 0;
				}
			}
			if (num2 > 0)
			{
				nnpColClues[m].Add(num2);
			}
			if (nnpColClues[m].Count == 0)
			{
				nnpColClues[m].Add(0);
			}
		}
	}

	private void NNCheck()
	{
		for (int i = 0; i < nnpH; i++)
		{
			for (int j = 0; j < nnpW; j++)
			{
				if (((nnpGrid[i, j] == 1) ? 1 : 0) != nnpSolution[i, j])
				{
					return;
				}
			}
		}
		nnpSolved = true;
	}

	private void ResetTT()
	{
		for (int i = 0; i < 9; i++)
		{
			ttb[i] = "";
		}
		ttWinner = 0;
		ttTurn = 0;
		ttLineA = -1;
		ttLineB = -1;
		ttAISym = (ttPlayerIsX ? "O" : "X");
		ttPlayerSym = (ttPlayerIsX ? "X" : "O");
	}

	private bool ttFull()
	{
		for (int i = 0; i < 9; i++)
		{
			if (ttb[i] == "")
			{
				return false;
			}
		}
		return true;
	}

	private void CheckTTWin()
	{
		int[,] array = new int[8, 3]
		{
			{ 0, 1, 2 },
			{ 3, 4, 5 },
			{ 6, 7, 8 },
			{ 0, 3, 6 },
			{ 1, 4, 7 },
			{ 2, 5, 8 },
			{ 0, 4, 8 },
			{ 2, 4, 6 }
		};
		for (int i = 0; i < 8; i++)
		{
			int num = array[i, 0];
			int num2 = array[i, 1];
			int num3 = array[i, 2];
			if (ttb[num] != "" && ttb[num] == ttb[num2] && ttb[num2] == ttb[num3])
			{
				ttWinner = ((ttb[num] == "X") ? 1 : 2);
				ttLineA = num;
				ttLineB = num3;
				if ((ttWinner == 1 && ttPlayerIsX) || (ttWinner == 2 && !ttPlayerIsX))
				{
					ttScoreX++;
				}
				else
				{
					ttScoreO++;
				}
				return;
			}
		}
		if (ttFull())
		{
			ttWinner = 3;
			ttScoreD++;
		}
	}

	private void TTAIMove()
	{
		if (ttWinner != 0)
		{
			return;
		}
		int num = -1;
		if (ttDiff != 0)
		{
			num = ((ttDiff != 1) ? TTHardAI() : TTMediumAI());
		}
		else
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 9; i++)
			{
				if (ttb[i] == "")
				{
					list.Add(i);
				}
			}
			if (list.Count > 0)
			{
				num = list[Random.Range(0, list.Count)];
			}
		}
		if (num >= 0 && num < 9 && ttb[num] == "")
		{
			ttb[num] = ttAISym;
			ttTurn = 0;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			CheckTTWin();
		}
	}

	private int TTMediumAI()
	{
		if (Random.Range(0, 3) == 0)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 9; i++)
			{
				if (ttb[i] == "")
				{
					list.Add(i);
				}
			}
			return (list.Count > 0) ? list[Random.Range(0, list.Count)] : (-1);
		}
		for (int j = 0; j < 9; j++)
		{
			if (ttb[j] == "")
			{
				ttb[j] = ttAISym;
				bool flag = TTCheckLine(ttAISym);
				ttb[j] = "";
				if (flag)
				{
					return j;
				}
			}
		}
		for (int k = 0; k < 9; k++)
		{
			if (ttb[k] == "")
			{
				ttb[k] = ttPlayerSym;
				bool flag2 = TTCheckLine(ttPlayerSym);
				ttb[k] = "";
				if (flag2)
				{
					return k;
				}
			}
		}
		List<int> list2 = new List<int>();
		for (int l = 0; l < 9; l++)
		{
			if (ttb[l] == "")
			{
				list2.Add(l);
			}
		}
		return (list2.Count > 0) ? list2[Random.Range(0, list2.Count)] : (-1);
	}

	private bool TTCheckLine(string player)
	{
		int[,] array = new int[8, 3]
		{
			{ 0, 1, 2 },
			{ 3, 4, 5 },
			{ 6, 7, 8 },
			{ 0, 3, 6 },
			{ 1, 4, 7 },
			{ 2, 5, 8 },
			{ 0, 4, 8 },
			{ 2, 4, 6 }
		};
		for (int i = 0; i < 8; i++)
		{
			int num = array[i, 0];
			int num2 = array[i, 1];
			int num3 = array[i, 2];
			if (ttb[num] == player && ttb[num2] == player && ttb[num3] == player)
			{
				return true;
			}
		}
		return false;
	}

	private int TTHardAI()
	{
		for (int i = 0; i < 9; i++)
		{
			if (ttb[i] == "")
			{
				ttb[i] = ttAISym;
				bool flag = TTCheckLine(ttAISym);
				ttb[i] = "";
				if (flag)
				{
					return i;
				}
			}
		}
		for (int j = 0; j < 9; j++)
		{
			if (ttb[j] == "")
			{
				ttb[j] = ttPlayerSym;
				bool flag2 = TTCheckLine(ttPlayerSym);
				ttb[j] = "";
				if (flag2)
				{
					return j;
				}
			}
		}
		int num = -100;
		int result = -1;
		for (int k = 0; k < 9; k++)
		{
			if (ttb[k] == "")
			{
				ttb[k] = ttAISym;
				int num2 = -TTMiniMaxHelper(isMax: false, -100, 100);
				ttb[k] = "";
				if (num2 > num)
				{
					num = num2;
					result = k;
				}
			}
		}
		return result;
	}

	private int TTMiniMaxHelper(bool isMax, int alpha, int beta)
	{
		if (TTCheckLine(ttAISym))
		{
			return 1;
		}
		if (TTCheckLine(ttPlayerSym))
		{
			return -1;
		}
		if (ttFull())
		{
			return 0;
		}
		if (isMax)
		{
			int num = -100;
			for (int i = 0; i < 9; i++)
			{
				if (ttb[i] == "")
				{
					ttb[i] = ttAISym;
					int val = TTMiniMaxHelper(isMax: false, alpha, beta);
					ttb[i] = "";
					num = Math.Max(num, val);
					alpha = Math.Max(alpha, val);
					if (beta <= alpha)
					{
						break;
					}
				}
			}
			return num;
		}
		int num2 = 100;
		for (int j = 0; j < 9; j++)
		{
			if (ttb[j] == "")
			{
				ttb[j] = ttPlayerSym;
				int val2 = TTMiniMaxHelper(isMax: true, alpha, beta);
				ttb[j] = "";
				num2 = Math.Min(num2, val2);
				beta = Math.Min(beta, val2);
				if (beta <= alpha)
				{
					break;
				}
			}
		}
		return num2;
	}

	private static Texture2D GetGradientTexture(Color top, Color bottom)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)gradientTexture != (Object)null)
		{
			return gradientTexture;
		}
		gradientTexture = new Texture2D(1, 32);
		((Object)gradientTexture).hideFlags = (HideFlags)61;
		for (int i = 0; i < 32; i++)
		{
			gradientTexture.SetPixel(0, i, Color.Lerp(top, bottom, (float)i / 31f));
		}
		gradientTexture.Apply();
		return gradientTexture;
	}

	private static Texture2D GetMenuIcon()
	{
		if ((Object)(object)menuIconTexture == (Object)null)
		{
			menuIconTexture = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.icon.png");
		}
		return menuIconTexture;
	}

	private void DrawRockPaperScissors()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Rock Paper Scissors</size>");
		GUI.Label(new Rect(170f, 48f, 400f, 20f), $"Wins: {rpsWins}  Losses: {rpsLosses}  Draws: {rpsDraws}  Streak: {rpsStreak}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "Reset"))
		{
			rpsWins = 0;
			rpsLosses = 0;
			rpsDraws = 0;
			rpsStreak = 0;
			rpsResult = "";
			rpsPlayerChoice = -1;
			rpsAIChoice = -1;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		string[] array = new string[3] { "Rock", "Paper", "Scissors" };
		Color[] array2 = (Color[])(object)new Color[3]
		{
			new Color(0.7f, 0.7f, 0.7f),
			new Color(0.3f, 0.6f, 0.9f),
			new Color(0.9f, 0.5f, 0.2f)
		};
		for (int i = 0; i < 3; i++)
		{
			GUI.backgroundColor = ((rpsPlayerChoice == i) ? array2[i] : guiColorB);
			if (GUI.Button(new Rect(220f + (float)i * 100f, 90f, 90f, 40f), array[i]))
			{
				rpsPlayerChoice = i;
				rpsAIChoice = Random.Range(0, 3);
				if (rpsPlayerChoice == rpsAIChoice)
				{
					rpsResult = "Draw!";
					rpsDraws++;
					rpsStreak = 0;
				}
				else if ((rpsPlayerChoice == 0 && rpsAIChoice == 2) || (rpsPlayerChoice == 1 && rpsAIChoice == 0) || (rpsPlayerChoice == 2 && rpsAIChoice == 1))
				{
					rpsResult = "You Win!";
					rpsWins++;
					rpsStreak = ((rpsStreak <= 0) ? 1 : (rpsStreak + 1));
				}
				else
				{
					rpsResult = "AI Wins!";
					rpsLosses++;
					rpsStreak = ((rpsStreak < 0) ? (rpsStreak - 1) : (-1));
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		if (rpsPlayerChoice >= 0 && rpsAIChoice >= 0)
		{
			GUI.Label(new Rect(170f, 150f, 200f, 25f), "<size=14>You: " + array[rpsPlayerChoice] + "  vs  AI: " + array[rpsAIChoice] + "</size>");
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 22,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = ((rpsResult == "You Win!") ? Color.green : ((rpsResult == "AI Wins!") ? Color.red : Color.yellow));
			GUI.Label(new Rect(200f, 190f, 230f, 40f), rpsResult, val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(170f, 250f, 400f, 20f), "<size=11>Pick Rock, Paper, or Scissors to play!</size>");
	}

	private void DrawNumberGuess()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Expected O, but got Unknown
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Expected O, but got Unknown
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		if (ngTarget == 0)
		{
			ngTarget = Random.Range(1, 101);
			ngAttempts = 0;
			ngWon = false;
			ngHint = "";
			ngInput = "";
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Number Guess</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), string.Format("Attempts: {0}  Best: {1}", ngAttempts, (ngBest < 0) ? "-" : ngBest.ToString()));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Number"))
		{
			ngTarget = Random.Range(1, 101);
			ngAttempts = 0;
			ngWon = false;
			ngHint = "";
			ngInput = "";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(200f, 90f, 300f, 25f), "<size=14>Guess a number between 1 and 100:</size>");
		ngInput = GUI.TextField(new Rect(200f, 120f, 150f, 25f), ngInput);
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(360f, 120f, 60f, 25f), "Guess") && !ngWon && int.TryParse(ngInput, out var result) && result >= 1 && result <= 100)
		{
			ngGuess = result;
			ngAttempts++;
			if (result == ngTarget)
			{
				ngWon = true;
				ngHint = "Correct!";
				if (ngBest < 0 || ngAttempts < ngBest)
				{
					ngBest = ngAttempts;
				}
			}
			else if (result < ngTarget)
			{
				ngHint = "Too low!";
			}
			else
			{
				ngHint = "Too high!";
			}
			ngInput = "";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 16,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		GUI.color = (ngWon ? Color.green : Color.yellow);
		GUI.Label(new Rect(200f, 160f, 250f, 30f), ngHint, val);
		GUI.color = Color.white;
		if (ngWon)
		{
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				alignment = (TextAnchor)4
			};
			GUI.Label(new Rect(200f, 200f, 250f, 25f), $"Got it in {ngAttempts} attempts!", val2);
		}
		GUI.Label(new Rect(170f, 260f, 400f, 20f), "<size=11>Type a number and click Guess</size>");
	}

	private void DrawDiceRoll()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		if (drDice[0] == 0 && drDice[1] == 0)
		{
			DRNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Dice Roll</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {drScore}  Best: {drBestScore}  Rerolls: {drRerolls}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			DRNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		for (int i = 0; i < 5; i++)
		{
			GUI.backgroundColor = (Color)(drHeld[i] ? new Color(0.8f, 0.6f, 0.1f) : guiColorB);
			if (GUI.Button(new Rect(200f + (float)i * 60f, 90f, 55f, 55f), drDice[i].ToString()) && drRolled)
			{
				drHeld[i] = !drHeld[i];
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 155f, 300f, 20f), "<size=11>Click dice to hold/unhold them</size>");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(220f, 185f, 100f, 30f), (drRerolls < 3) ? "Roll" : "Score"))
		{
			if (drRerolls < 3)
			{
				for (int j = 0; j < 5; j++)
				{
					if (!drHeld[j])
					{
						drDice[j] = Random.Range(1, 7);
					}
				}
				drRerolls++;
				drRolled = true;
			}
			else
			{
				drScore += DRCalculateScore();
				if (drScore > drBestScore)
				{
					drBestScore = drScore;
				}
				DRNewGame();
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 230f, 400f, 20f), $"<size=11>Turn Score: {DRCalculateScore()}  |  Roll up to 3 times per turn</size>");
	}

	private void DRNewGame()
	{
		for (int i = 0; i < 5; i++)
		{
			drDice[i] = Random.Range(1, 7);
			drHeld[i] = false;
		}
		drRerolls = 0;
		drRolled = false;
	}

	private int DRCalculateScore()
	{
		int[] array = new int[7];
		for (int i = 0; i < 5; i++)
		{
			array[drDice[i]]++;
		}
		int num = 0;
		if (array.Contains(5))
		{
			num = 50;
		}
		else if (array.Contains(4))
		{
			num = 40;
		}
		else if (array.Contains(3) && array.Contains(2))
		{
			num = 25;
		}
		else if (array.Contains(3))
		{
			num = 15;
		}
		else if (array.Where((int c) => c >= 2).Count() >= 2)
		{
			num = 10;
		}
		else
		{
			for (int num2 = 1; num2 <= 6; num2++)
			{
				num += array[num2] * num2;
			}
		}
		return num;
	}

	private void DrawCoinFlip()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Expected O, but got Unknown
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Coin Flip</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Total Flips: {cfTotal}  |  Streak: {Mathf.Abs(cfStreak)} {cfStreakType}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "Reset"))
		{
			cfResult = -1;
			cfStreak = 0;
			cfTotal = 0;
			cfStreakType = "";
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.backgroundColor = new Color(0.9f, 0.8f, 0.2f);
		if (GUI.Button(new Rect(250f, 100f, 120f, 120f), (cfResult < 0) ? "Flip!" : ((cfResult == 0) ? "HEADS" : "TAILS")))
		{
			cfResult = Random.Range(0, 2);
			cfTotal++;
			string text = ((cfResult == 0) ? "Heads" : "Tails");
			if (cfStreakType == text || cfStreak == 0)
			{
				cfStreak = ((cfStreak < 0) ? 1 : (cfStreak + 1));
				cfStreakType = text;
			}
			else
			{
				cfStreak = ((cfStreak <= 0) ? 1 : (-1));
				cfStreakType = text;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 18,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		GUI.color = ((cfStreakType == "Heads") ? new Color(1f, 0.85f, 0.2f) : new Color(0.7f, 0.7f, 0.7f));
		GUI.Label(new Rect(200f, 240f, 230f, 30f), (cfResult >= 0) ? (cfStreakType + "!") : "", val);
		GUI.color = Color.white;
		GUIStyle val2 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 14,
			alignment = (TextAnchor)4
		};
		GUI.Label(new Rect(200f, 280f, 230f, 25f), (Mathf.Abs(cfStreak) > 1) ? $"Streak of {Mathf.Abs(cfStreak)}!" : "", val2);
		GUI.Label(new Rect(170f, 330f, 400f, 20f), "<size=11>Click the coin to flip it</size>");
	}

	private void DrawBlackjack()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Expected O, but got Unknown
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		if (bjPlayerHand.Count == 0)
		{
			bjChips = 1000;
			bjBetting = true;
			bjBet = 100;
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Blackjack</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Chips: {bjChips}  |  Bet: {bjBet}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Hand"))
		{
			bjBetting = true;
			bjGameOver = false;
			bjResult = "";
			bjPlayerHand.Clear();
			bjDealerHand.Clear();
			bjPlayerLabels.Clear();
			bjDealerLabels.Clear();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		if (bjBetting)
		{
			GUI.Label(new Rect(200f, 90f, 200f, 25f), "<size=14>Place your bet:</size>");
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(200f, 120f, 60f, 25f), "-50"))
			{
				bjBet = Mathf.Max(50, bjBet - 50);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			GUI.Label(new Rect(270f, 120f, 80f, 25f), bjBet.ToString());
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(360f, 120f, 60f, 25f), "+50"))
			{
				bjBet = Mathf.Min(bjChips, bjBet + 50);
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(260f, 160f, 100f, 30f), "Deal") && bjBet > 0 && bjBet <= bjChips)
			{
				bjChips -= bjBet;
				bjPlayerHand.Clear();
				bjDealerHand.Clear();
				bjPlayerLabels.Clear();
				bjDealerLabels.Clear();
				bjPlayerHand.Add(BJDrawCard());
				bjPlayerHand.Add(BJDrawCard());
				bjDealerHand.Add(BJDrawCard());
				bjDealerHand.Add(BJDrawCard());
				for (int i = 0; i < bjPlayerHand.Count; i++)
				{
					bjPlayerLabels.Add(BJCardName(bjPlayerHand[i]));
				}
				for (int j = 0; j < bjDealerHand.Count; j++)
				{
					bjDealerLabels.Add(BJCardName(bjDealerHand[j]));
				}
				bjDealerHidden = true;
				bjGameOver = false;
				bjBetting = false;
				bjResult = "";
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
			GUI.Label(new Rect(200f, 200f, 300f, 20f), "<size=11>Min bet: 50 chips</size>");
		}
		else
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				alignment = (TextAnchor)4,
				richText = true
			};
			int num = ((bjDealerHand[0] == 1) ? 11 : ((bjDealerHand[0] >= 10) ? 10 : bjDealerHand[0]));
			GUI.Label(new Rect(170f, 75f, 200f, 20f), "<size=12>Dealer (" + (bjDealerHidden ? (num + " + ?") : BJHandValue(bjDealerHand).ToString()) + ")</size>");
			for (int k = 0; k < bjDealerHand.Count; k++)
			{
				GUI.backgroundColor = new Color(0.1f, 0.5f, 0.1f);
				if (k == 1 && bjDealerHidden)
				{
					GUI.Box(new Rect(180f + (float)k * 50f, 100f, 45f, 60f), "?");
					continue;
				}
				GUI.backgroundColor = new Color(0.95f, 0.95f, 0.9f);
				GUI.Box(new Rect(180f + (float)k * 50f, 100f, 45f, 60f), bjDealerLabels[k]);
			}
			GUI.backgroundColor = guiColorB;
			GUI.Label(new Rect(170f, 175f, 200f, 20f), $"<size=12>Your Hand ({BJHandValue(bjPlayerHand)})</size>");
			for (int l = 0; l < bjPlayerHand.Count; l++)
			{
				GUI.backgroundColor = new Color(0.95f, 0.95f, 0.9f);
				GUI.Box(new Rect(180f + (float)l * 50f, 200f, 45f, 60f), bjPlayerLabels[l]);
			}
			if (!bjGameOver)
			{
				GUI.backgroundColor = guiColorA;
				if (GUI.Button(new Rect(200f, 280f, 80f, 30f), "Hit"))
				{
					bjPlayerHand.Add(BJDrawCard());
					bjPlayerLabels.Add(BJCardName(bjPlayerHand[bjPlayerHand.Count - 1]));
					if (BJHandValue(bjPlayerHand) > 21)
					{
						bjResult = "Bust! You lose.";
						bjGameOver = true;
					}
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = new Color(0.8f, 0.3f, 0.3f);
				if (GUI.Button(new Rect(310f, 280f, 80f, 30f), "Stand"))
				{
					bjDealerHidden = false;
					while (BJHandValue(bjDealerHand) < 17)
					{
						bjDealerHand.Add(BJDrawCard());
						bjDealerLabels.Add(BJCardName(bjDealerHand[bjDealerHand.Count - 1]));
					}
					int num2 = BJHandValue(bjDealerHand);
					int num3 = BJHandValue(bjPlayerHand);
					if (num2 > 21)
					{
						bjResult = "Dealer busts! You win!";
						bjChips += bjBet * 2;
					}
					else if (num3 > num2)
					{
						bjResult = "You win!";
						bjChips += bjBet * 2;
					}
					else if (num3 < num2)
					{
						bjResult = "Dealer wins.";
					}
					else
					{
						bjResult = "Push!";
						bjChips += bjBet;
					}
					bjGameOver = true;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				GUI.backgroundColor = guiColorB;
			}
			if (bjGameOver)
			{
				GUIStyle val2 = new GUIStyle(GUI.skin.label)
				{
					fontSize = 16,
					fontStyle = (FontStyle)1,
					alignment = (TextAnchor)4
				};
				GUI.color = ((bjResult.Contains("win") || bjResult.Contains("Win")) ? Color.green : Color.red);
				GUI.Label(new Rect(200f, 325f, 200f, 25f), bjResult, val2);
				GUI.color = Color.white;
				GUI.Label(new Rect(200f, 355f, 200f, 20f), $"Chips: {bjChips}");
			}
		}
		GUI.Label(new Rect(170f, 390f, 400f, 20f), "<size=11>Get as close to 21 as possible without going over!</size>");
	}

	private int BJDrawCard()
	{
		return Random.Range(1, 14);
	}

	private int BJHandValue(List<int> hand)
	{
		int num = 0;
		int num2 = 0;
		foreach (int item in hand)
		{
			if (item == 1)
			{
				num2++;
				num += 11;
			}
			else
			{
				num = ((item < 10) ? (num + item) : (num + 10));
			}
		}
		while (num > 21 && num2 > 0)
		{
			num -= 10;
			num2--;
		}
		return num;
	}

	private string BJCardName(int c)
	{
		string[] array = new string[13]
		{
			"A", "2", "3", "4", "5", "6", "7", "8", "9", "10",
			"J", "Q", "K"
		};
		return array[Mathf.Clamp(c, 1, 13) - 1];
	}

	private void DrawGomoku()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (gmBoard == null)
		{
			GMNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Gomoku (Five in a Row)</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), (gmWinner != 0) ? ((gmWinner == 1) ? "You win!" : "AI wins!") : ((gmTurn == 0) ? "Your turn" : "AI thinking..."));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			GMNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 20f;
		float num2 = 190f;
		float num3 = 75f;
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				GUI.backgroundColor = (Color)((gmBoard[i, j] == 0) ? new Color(0.35f, 0.25f, 0.15f) : ((gmBoard[i, j] == 1) ? Color.white : Color.black));
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "") && gmBoard[i, j] == 0 && gmTurn == 0 && gmWinner == 0 && !gmAIThinking)
				{
					gmBoard[i, j] = 1;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					if (GMCheckWin(1))
					{
						gmWinner = 1;
						continue;
					}
					if (GMFull())
					{
						gmWinner = 3;
						continue;
					}
					gmTurn = 1;
					gmAIThinking = true;
					((MonoBehaviour)this).Invoke("GMAIMove", 0.2f);
				}
			}
		}
		GUI.backgroundColor = guiColorB;
	}

	private void GMNewGame()
	{
		gmBoard = new int[15, 15];
		gmTurn = 0;
		gmWinner = 0;
		gmAIThinking = false;
		gmWinR1 = (gmWinC1 = (gmWinR2 = (gmWinC2 = -1)));
	}

	private bool GMCheckWin(int p)
	{
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 11; j++)
			{
				if (gmBoard[i, j] == p && gmBoard[i, j + 1] == p && gmBoard[i, j + 2] == p && gmBoard[i, j + 3] == p && gmBoard[i, j + 4] == p)
				{
					return true;
				}
			}
		}
		for (int k = 0; k < 11; k++)
		{
			for (int l = 0; l < 15; l++)
			{
				if (gmBoard[k, l] == p && gmBoard[k + 1, l] == p && gmBoard[k + 2, l] == p && gmBoard[k + 3, l] == p && gmBoard[k + 4, l] == p)
				{
					return true;
				}
			}
		}
		for (int m = 0; m < 11; m++)
		{
			for (int n = 0; n < 11; n++)
			{
				if (gmBoard[m, n] == p && gmBoard[m + 1, n + 1] == p && gmBoard[m + 2, n + 2] == p && gmBoard[m + 3, n + 3] == p && gmBoard[m + 4, n + 4] == p)
				{
					return true;
				}
			}
		}
		for (int num = 4; num < 15; num++)
		{
			for (int num2 = 0; num2 < 11; num2++)
			{
				if (gmBoard[num, num2] == p && gmBoard[num - 1, num2 + 1] == p && gmBoard[num - 2, num2 + 2] == p && gmBoard[num - 3, num2 + 3] == p && gmBoard[num - 4, num2 + 4] == p)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool GMFull()
	{
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				if (gmBoard[i, j] == 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void GMAIMove()
	{
		gmAIThinking = false;
		if (gmWinner != 0)
		{
			return;
		}
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				if (gmBoard[i, j] == 0)
				{
					int num4 = GMEval(i, j, 2) + GMEval(i, j, 1) / 2;
					if (num4 > num3)
					{
						num3 = num4;
						num = i;
						num2 = j;
					}
				}
			}
		}
		if (num >= 0)
		{
			gmBoard[num, num2] = 2;
			if (GMCheckWin(2))
			{
				gmWinner = 2;
			}
			else if (GMFull())
			{
				gmWinner = 3;
			}
			else
			{
				gmTurn = 0;
			}
		}
	}

	private int GMEval(int r, int c, int p)
	{
		int num = 0;
		int[][] array = new int[4][]
		{
			new int[2] { 0, 1 },
			new int[2] { 1, 0 },
			new int[2] { 1, 1 },
			new int[2] { -1, 1 }
		};
		int[][] array2 = array;
		foreach (int[] array3 in array2)
		{
			int num2 = 0;
			for (int j = 1; j <= 4; j++)
			{
				int num3 = r + array3[0] * j;
				int num4 = c + array3[1] * j;
				if (num3 >= 0 && num3 < 15 && num4 >= 0 && num4 < 15 && gmBoard[num3, num4] == p)
				{
					num2++;
					continue;
				}
				break;
			}
			for (int k = 1; k <= 4; k++)
			{
				int num5 = r - array3[0] * k;
				int num6 = c - array3[1] * k;
				if (num5 >= 0 && num5 < 15 && num6 >= 0 && num6 < 15 && gmBoard[num5, num6] == p)
				{
					num2++;
					continue;
				}
				break;
			}
			if (num2 >= 4)
			{
				num += 10000;
				continue;
			}
			switch (num2)
			{
			case 3:
				num += 1000;
				break;
			case 2:
				num += 100;
				break;
			case 1:
				num += 10;
				break;
			}
		}
		return num;
	}

	private void DrawDotsAndBoxes()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Expected O, but got Unknown
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		if (dbHoriz == null)
		{
			DBNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Dots and Boxes</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), string.Format("P1: {0}  |  P2: {1}  |  {2}", dbScore1, dbScore2, (dbTurn == 0) ? "P1's turn" : "P2's turn"));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			DBNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 40f;
		float num2 = 220f;
		float num3 = 85f;
		for (int i = 0; i <= dbRows; i++)
		{
			for (int j = 0; j <= dbCols; j++)
			{
				GUI.backgroundColor = new Color(0.8f, 0.7f, 0.3f);
				GUI.Box(new Rect(num2 + (float)j * num - 4f, num3 + (float)i * num - 4f, 8f, 8f), "");
			}
		}
		for (int k = 0; k < dbRows; k++)
		{
			for (int l = 0; l < dbCols; l++)
			{
				if (dbBoxes[k, l] > 0)
				{
					GUI.backgroundColor = ((dbBoxes[k, l] == 1) ? new Color(0.3f, 0.5f, 1f, 0.3f) : new Color(1f, 0.3f, 0.3f, 0.3f));
					GUI.Box(new Rect(num2 + (float)l * num + 6f, num3 + (float)k * num + 6f, num - 12f, num - 12f), (dbBoxes[k, l] == 1) ? "P1" : "P2");
				}
			}
		}
		for (int m = 0; m <= dbRows; m++)
		{
			for (int n = 0; n < dbCols; n++)
			{
				bool flag = dbHoriz[m, n];
				GUI.backgroundColor = ((!flag) ? new Color(0.3f, 0.3f, 0.35f) : ((dbTurn == 0) ? new Color(0.3f, 0.5f, 1f) : new Color(1f, 0.3f, 0.3f)));
				if (GUI.Button(new Rect(num2 + (float)n * num + 8f, num3 + (float)m * num - 3f, num - 16f, 6f), flag ? "" : "") && !flag && !dbGameOver)
				{
					dbHoriz[m, n] = true;
					if (!DBCheckBox())
					{
						dbTurn = 1 - dbTurn;
					}
					DBCheckDone();
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
		}
		for (int num4 = 0; num4 < dbRows; num4++)
		{
			for (int num5 = 0; num5 <= dbCols; num5++)
			{
				bool flag2 = dbVert[num4, num5];
				GUI.backgroundColor = ((!flag2) ? new Color(0.3f, 0.3f, 0.35f) : ((dbTurn == 0) ? new Color(0.3f, 0.5f, 1f) : new Color(1f, 0.3f, 0.3f)));
				if (GUI.Button(new Rect(num2 + (float)num5 * num - 3f, num3 + (float)num4 * num + 8f, 6f, num - 16f), flag2 ? "" : "") && !flag2 && !dbGameOver)
				{
					dbVert[num4, num5] = true;
					if (!DBCheckBox())
					{
						dbTurn = 1 - dbTurn;
					}
					DBCheckDone();
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (dbGameOver)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			string text = ((dbScore1 > dbScore2) ? "P1 wins!" : ((dbScore2 > dbScore1) ? "P2 wins!" : "Draw!"));
			GUI.color = Color.yellow;
			GUI.Label(new Rect(200f, num3 + (float)(dbRows + 1) * num + 20f, 230f, 30f), text, val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(170f, 380f, 400f, 20f), "<size=11>Click edges between dots. Complete a box = go again!</size>");
	}

	private void DBNewGame()
	{
		dbHoriz = new bool[dbRows + 1, dbCols];
		dbVert = new bool[dbRows, dbCols + 1];
		dbBoxes = new int[dbRows, dbCols];
		dbTurn = 0;
		dbScore1 = 0;
		dbScore2 = 0;
		dbGameOver = false;
	}

	private bool DBCheckBox()
	{
		bool result = false;
		for (int i = 0; i < dbRows; i++)
		{
			for (int j = 0; j < dbCols; j++)
			{
				if (dbBoxes[i, j] == 0 && dbHoriz[i, j] && dbHoriz[i + 1, j] && dbVert[i, j] && dbVert[i, j + 1])
				{
					dbBoxes[i, j] = dbTurn + 1;
					if (dbTurn == 0)
					{
						dbScore1++;
					}
					else
					{
						dbScore2++;
					}
					result = true;
				}
			}
		}
		return result;
	}

	private void DBCheckDone()
	{
		int num = 0;
		for (int i = 0; i < dbRows; i++)
		{
			for (int j = 0; j < dbCols; j++)
			{
				if (dbBoxes[i, j] > 0)
				{
					num++;
				}
			}
		}
		if (num == dbRows * dbCols)
		{
			dbGameOver = true;
		}
	}

	private void DrawCheckers2P()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		if (ck2Board == null)
		{
			CK2NewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Checkers 2P</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), (!ck2GameOver) ? ((ck2Turn == 0) ? "Red's turn" : "Black's turn") : ((ck2Winner == 1) ? "Red wins!" : "Black wins!"));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			CK2NewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 42f;
		float num2 = 210f;
		float num3 = 80f;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				GUI.backgroundColor = (((i + j) % 2 == 1) ? new Color(0.3f, 0.2f, 0.1f) : new Color(0.8f, 0.7f, 0.5f));
				if (GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), "") && !ck2GameOver)
				{
					if (ck2Board[i, j] != 0 && (ck2Board[i, j] == 1 || ck2Board[i, j] == 2) && ((ck2Board[i, j] == 1 && ck2Turn == 0) || (ck2Board[i, j] == 2 && ck2Turn == 1)))
					{
						ck2SelR = i;
						ck2SelC = j;
					}
					else if (ck2SelR >= 0 && ck2Board[i, j] == 0)
					{
						int num6 = i - ck2SelR;
						int num7 = j - ck2SelC;
						int num8 = ck2Board[ck2SelR, ck2SelC];
						bool flag = false;
						if (num8 == 1 && num6 == -1 && Mathf.Abs(num7) == 1)
						{
							flag = true;
						}
						if (num8 == 2 && num6 == 1 && Mathf.Abs(num7) == 1)
						{
							flag = true;
						}
						if (Mathf.Abs(num6) == 2 && Mathf.Abs(num7) == 2)
						{
							int num9 = (ck2SelR + i) / 2;
							int num10 = (ck2SelC + j) / 2;
							if (ck2Board[num9, num10] != 0 && ck2Board[num9, num10] != num8)
							{
								flag = true;
							}
						}
						if (flag)
						{
							if (Mathf.Abs(num6) == 2)
							{
								int num11 = (ck2SelR + i) / 2;
								int num12 = (ck2SelC + j) / 2;
								ck2Board[num11, num12] = 0;
							}
							ck2Board[i, j] = num8;
							ck2Board[ck2SelR, ck2SelC] = 0;
							if (num8 == 1 && i == 0)
							{
								ck2Board[i, j] = 3;
							}
							if (num8 == 2 && i == 7)
							{
								ck2Board[i, j] = 4;
							}
							ck2Turn = 1 - ck2Turn;
							ck2SelR = (ck2SelC = -1);
							CK2CheckWin();
						}
					}
					else
					{
						ck2SelR = (ck2SelC = -1);
					}
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				int num13 = ck2Board[i, j];
				if (num13 == 1 || num13 == 3)
				{
					GUI.backgroundColor = (Color)((num13 == 3) ? new Color(1f, 0.8f, 0f) : Color.red);
					GUI.Label(new Rect(num4 + 10f, num5 + 8f, 20f, 20f), (num13 == 3) ? "K" : "O");
				}
				else if (num13 == 2 || num13 == 4)
				{
					GUI.backgroundColor = (Color)((num13 == 4) ? new Color(0.8f, 0.8f, 1f) : Color.black);
					GUI.Label(new Rect(num4 + 10f, num5 + 8f, 20f, 20f), (num13 == 4) ? "K" : "O");
				}
				if (ck2SelR == i && ck2SelC == j)
				{
					GUI.color = Color.yellow;
					GUI.DrawTexture(new Rect(num4, num5, num - 1f, num - 1f), (Texture)(object)Texture2D.whiteTexture);
					GUI.color = Color.white;
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 430f, 400f, 20f), "<size=11>Click a piece, then click where to move it</size>");
	}

	private void CK2NewGame()
	{
		ck2Board = new int[8, 8];
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if ((i + j) % 2 == 1 && i < 3)
				{
					ck2Board[i, j] = 2;
				}
				else if ((i + j) % 2 == 1 && i > 4)
				{
					ck2Board[i, j] = 1;
				}
				else
				{
					ck2Board[i, j] = 0;
				}
			}
		}
		ck2Turn = 0;
		ck2SelR = (ck2SelC = -1);
		ck2GameOver = false;
		ck2Winner = 0;
	}

	private void CK2CheckWin()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (ck2Board[i, j] == 1 || ck2Board[i, j] == 3)
				{
					flag = true;
				}
				if (ck2Board[i, j] == 2 || ck2Board[i, j] == 4)
				{
					flag2 = true;
				}
			}
		}
		if (!flag)
		{
			ck2GameOver = true;
			ck2Winner = 2;
		}
		else if (!flag2)
		{
			ck2GameOver = true;
			ck2Winner = 1;
		}
	}

	private void DrawSlidingPuzzle()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Expected O, but got Unknown
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (spGrid == null)
		{
			SPNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Sliding Puzzle</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), string.Format("Moves: {0}  Best: {1}", spMoves, (spBest < 0) ? "-" : spBest.ToString()));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			SPNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 60f;
		float num2 = 220f;
		float num3 = 85f;
		GUIStyle val = new GUIStyle(GUI.skin.button)
		{
			fontSize = 20,
			fontStyle = (FontStyle)1
		};
		for (int i = 0; i < spSize; i++)
		{
			for (int j = 0; j < spSize; j++)
			{
				int num4 = spGrid[i, j];
				GUI.backgroundColor = ((num4 == 0) ? new Color(0.2f, 0.2f, 0.25f) : new Color(0.3f, 0.5f, 0.9f));
				if (num4 <= 0 || !GUI.Button(new Rect(num2 + (float)j * num, num3 + (float)i * num, num - 2f, num - 2f), num4.ToString(), val) || spSolved)
				{
					continue;
				}
				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						if (Mathf.Abs(k) + Mathf.Abs(l) == 1)
						{
							int num5 = i + k;
							int num6 = j + l;
							if (num5 >= 0 && num5 < spSize && num6 >= 0 && num6 < spSize && spGrid[num5, num6] == 0)
							{
								spGrid[num5, num6] = num4;
								spGrid[i, j] = 0;
								spMoves++;
								SPCheckSolved();
								SoundManager.Play(SoundManager.DefaultSounds["Button"]);
								return;
							}
						}
					}
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (spSolved)
		{
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = Color.green;
			GUI.Label(new Rect(200f, num3 + (float)spSize * num + 15f, 230f, 30f), $"Solved in {spMoves} moves!", val2);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(170f, 380f, 400f, 20f), "<size=11>Click a tile adjacent to the empty space to slide it</size>");
	}

	private void SPNewGame()
	{
		spGrid = new int[spSize, spSize];
		int num = 1;
		for (int i = 0; i < spSize; i++)
		{
			for (int j = 0; j < spSize; j++)
			{
				spGrid[i, j] = num++;
			}
		}
		spGrid[spSize - 1, spSize - 1] = 0;
		spMoves = 0;
		spSolved = false;
		for (int k = 0; k < 200; k++)
		{
			List<int[]> list = new List<int[]>();
			for (int l = 0; l < spSize; l++)
			{
				for (int m = 0; m < spSize; m++)
				{
					if (spGrid[l, m] == 0)
					{
						list.Add(new int[2] { l, m });
					}
				}
			}
			int num2 = list[0][0];
			int num3 = list[0][1];
			List<int[]> list2 = new List<int[]>();
			if (num2 > 0)
			{
				list2.Add(new int[2]
				{
					num2 - 1,
					num3
				});
			}
			if (num2 < spSize - 1)
			{
				list2.Add(new int[2]
				{
					num2 + 1,
					num3
				});
			}
			if (num3 > 0)
			{
				list2.Add(new int[2]
				{
					num2,
					num3 - 1
				});
			}
			if (num3 < spSize - 1)
			{
				list2.Add(new int[2]
				{
					num2,
					num3 + 1
				});
			}
			int[] array = list2[Random.Range(0, list2.Count)];
			spGrid[num2, num3] = spGrid[array[0], array[1]];
			spGrid[array[0], array[1]] = 0;
		}
	}

	private void SPCheckSolved()
	{
		int num = 1;
		for (int i = 0; i < spSize; i++)
		{
			for (int j = 0; j < spSize; j++)
			{
				if (i == spSize - 1 && j == spSize - 1)
				{
					if (spGrid[i, j] != 0)
					{
						return;
					}
					continue;
				}
				if (spGrid[i, j] != num)
				{
					return;
				}
				num++;
			}
		}
		spSolved = true;
		if (spBest < 0 || spMoves < spBest)
		{
			spBest = spMoves;
		}
	}

	private void DrawBullsAndCows()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Expected O, but got Unknown
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Expected O, but got Unknown
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		if (bucSecret[0] == 0 && bucSecret[1] == 0 && bucSecret[2] == 0 && bucSecret[3] == 0)
		{
			BUCNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Bulls and Cows</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Attempt {bucAttempt}/{bucMaxAttempts}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			BUCNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			fontSize = 12,
			richText = true
		};
		float num = 75f;
		foreach (string item in bucHistory)
		{
			GUI.Label(new Rect(200f, num, 300f, 18f), item, val);
			num += 18f;
		}
		if (!bucWon && bucAttempt < bucMaxAttempts)
		{
			GUI.Label(new Rect(200f, num + 10f, 200f, 20f), "<size=12>Enter 4 digits (0-9):</size>");
			bucInput = GUI.TextField(new Rect(200f, num + 35f, 120f, 25f), bucInput);
			GUI.backgroundColor = guiColorA;
			if (GUI.Button(new Rect(330f, num + 35f, 60f, 25f), "Guess") && bucInput.Length == 4 && bucInput.All(char.IsDigit))
			{
				int[] array = bucInput.Select((char c) => c - 48).ToArray();
				int num2 = 0;
				int num3 = 0;
				for (int num4 = 0; num4 < 4; num4++)
				{
					if (array[num4] == bucSecret[num4])
					{
						num2++;
					}
					else if (bucSecret.Contains(array[num4]))
					{
						num3++;
					}
				}
				bucAttempt++;
				bucHistory.Add($"{bucInput}  ->  {num2}B {num3}C");
				if (num2 == 4)
				{
					bucWon = true;
				}
				bucInput = "";
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
			GUI.backgroundColor = guiColorB;
		}
		if (bucWon)
		{
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				fontStyle = (FontStyle)1
			};
			GUI.color = Color.green;
			GUI.Label(new Rect(200f, 350f, 300f, 25f), $"You got it in {bucAttempt} attempts!", val2);
			GUI.color = Color.white;
		}
		else if (bucAttempt >= bucMaxAttempts)
		{
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				fontStyle = (FontStyle)1
			};
			GUI.color = Color.red;
			GUI.Label(new Rect(200f, 350f, 300f, 25f), "Game Over! Code was: " + string.Join("", bucSecret), val3);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(170f, 390f, 400f, 20f), "<size=11>Bulls = right digit, right spot | Cows = right digit, wrong spot</size>");
	}

	private void BUCNewGame()
	{
		bucSecret = new int[4]
		{
			Random.Range(1, 10),
			Random.Range(0, 10),
			Random.Range(0, 10),
			Random.Range(0, 10)
		};
		bucAttempt = 0;
		bucWon = false;
		bucInput = "";
		bucHistory = new List<string>();
	}

	private void DrawFreeCell()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Expected O, but got Unknown
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Expected O, but got Unknown
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		if (fcColumns == null)
		{
			FCNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>FreeCell</size>");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			FCNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 40f;
		float num2 = 55f;
		for (int i = 0; i < 4; i++)
		{
			GUI.backgroundColor = new Color(0.2f, 0.5f, 0.2f);
			GUI.Box(new Rect(185f + (float)i * (num + 5f), 80f, num, num2), (fcFoundation[i].Count > 0) ? BJCardName(fcFoundation[i][fcFoundation[i].Count - 1]) : "A");
		}
		for (int j = 0; j < 8; j++)
		{
			float num3 = 185f + (float)j * (num + 5f);
			GUI.backgroundColor = ((fcSelectedCol == j) ? new Color(0.5f, 0.5f, 0.2f) : new Color(0.3f, 0.3f, 0.35f));
			if (GUI.Button(new Rect(num3, 145f, num, num2 - 10f), ""))
			{
				if (fcSelectedCol < 0 && fcColumns[j].Count > 0)
				{
					fcSelectedCol = j;
					fcSelectedIdx = fcColumns[j].Count - 1;
				}
				else if (fcSelectedCol == j)
				{
					fcSelectedCol = -1;
					fcSelectedIdx = -1;
				}
				else if (fcSelectedCol >= 0)
				{
					FCMoveToColumn(j);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
			for (int k = 0; k < fcColumns[j].Count; k++)
			{
				int c = fcColumns[j][k];
				bool flag = fcColFaceUp[j][k];
				GUI.backgroundColor = (flag ? new Color(0.95f, 0.95f, 0.9f) : new Color(0.2f, 0.2f, 0.6f));
				GUIStyle val = new GUIStyle(GUI.skin.label)
				{
					fontSize = 10,
					alignment = (TextAnchor)4
				};
				GUI.Label(new Rect(num3 + 2f, 145f + (float)(k * 14), num - 4f, 14f), flag ? BJCardName(c) : "?", val);
			}
		}
		GUI.backgroundColor = guiColorB;
		if (FCCheckWin())
		{
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = Color.green;
			GUI.Label(new Rect(200f, 350f, 250f, 30f), "You Win!", val2);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(170f, 390f, 400f, 20f), "<size=11>Click a column to select, then click another to move</size>");
	}

	private void FCNewGame()
	{
		List<int> list = new List<int>();
		for (int i = 1; i <= 52; i++)
		{
			list.Add(i);
		}
		for (int num = list.Count - 1; num > 0; num--)
		{
			int index = Random.Range(0, num + 1);
			int value = list[num];
			list[num] = list[index];
			list[index] = value;
		}
		fcColumns = new List<int>[8];
		fcColFaceUp = new List<bool>[8];
		fcFoundation = new List<int>[4];
		for (int j = 0; j < 8; j++)
		{
			fcColumns[j] = new List<int>();
			fcColFaceUp[j] = new List<bool>();
		}
		for (int k = 0; k < 4; k++)
		{
			fcFoundation[k] = new List<int>();
		}
		int num2 = 0;
		for (int l = 0; l < 8; l++)
		{
			int num3 = ((l < 4) ? 7 : 6);
			for (int m = 0; m < num3; m++)
			{
				fcColumns[l].Add(list[num2]);
				fcColFaceUp[l].Add(m == num3 - 1);
				num2++;
			}
		}
		fcSelectedCol = -1;
		fcSelectedIdx = -1;
	}

	private void FCMoveToColumn(int target)
	{
		if (fcSelectedCol < 0)
		{
			return;
		}
		int num = fcColumns[fcSelectedCol][fcColumns[fcSelectedCol].Count - 1];
		int num2 = (num - 1) / 13;
		int num3 = (num - 1) % 13;
		if (fcColumns[target].Count > 0)
		{
			int num4 = fcColumns[target][fcColumns[target].Count - 1];
			int num5 = (num4 - 1) / 13;
			int num6 = (num4 - 1) % 13;
			if (num5 % 2 == num2 % 2 || num3 != num6 - 1)
			{
				fcSelectedCol = -1;
				return;
			}
		}
		fcColumns[target].Add(num);
		fcColFaceUp[target].Add(item: true);
		fcColumns[fcSelectedCol].RemoveAt(fcColumns[fcSelectedCol].Count - 1);
		if (fcColumns[fcSelectedCol].Count > 0)
		{
			fcColFaceUp[fcSelectedCol][fcColFaceUp[fcSelectedCol].Count - 1] = true;
		}
		fcSelectedCol = -1;
		fcSelectedIdx = -1;
	}

	private bool FCCheckWin()
	{
		for (int i = 0; i < 4; i++)
		{
			if (fcFoundation[i].Count < 13)
			{
				return false;
			}
		}
		return true;
	}

	private void DrawTron()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		if (trGrid == null)
		{
			TRNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Tron / Light Cycles</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), trActive ? "Use WASD or Arrow Keys" : (trAlive ? "You crashed!" : "AI crashed! You win!"));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			TRNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 16f;
		float num2 = 190f;
		float num3 = 75f;
		if (trActive)
		{
			if (Input.GetKey((KeyCode)119) || Input.GetKey((KeyCode)273))
			{
				trPDir = 0;
			}
			if (Input.GetKey((KeyCode)115) || Input.GetKey((KeyCode)274))
			{
				trPDir = 2;
			}
			if (Input.GetKey((KeyCode)97) || Input.GetKey((KeyCode)276))
			{
				trPDir = 3;
			}
			if (Input.GetKey((KeyCode)100) || Input.GetKey((KeyCode)275))
			{
				trPDir = 1;
			}
			int[] array = new int[4] { -1, 0, 1, 0 };
			int[] array2 = new int[4] { 0, 1, 0, -1 };
			int num4 = trPR + array[trPDir];
			int num5 = trPC + array2[trPDir];
			if (num4 < 0 || num4 >= trSize || num5 < 0 || num5 >= trSize || trGrid[num4, num5] != 0)
			{
				trActive = false;
				trAlive = false;
			}
			else
			{
				trPR = num4;
				trPC = num5;
				trGrid[trPR, trPC] = 1;
			}
			int num6 = trER + array[trEDir];
			int num7 = trEC + array2[trEDir];
			if (num6 < 0 || num6 >= trSize || num7 < 0 || num7 >= trSize || trGrid[num6, num7] != 0)
			{
				int[] array3 = new int[4] { 0, 1, 2, 3 };
				List<int> list = new List<int>();
				int[] array4 = array3;
				foreach (int num8 in array4)
				{
					int num9 = trER + array[num8];
					int num10 = trEC + array2[num8];
					if (num9 >= 0 && num9 < trSize && num10 >= 0 && num10 < trSize && trGrid[num9, num10] == 0)
					{
						list.Add(num8);
					}
				}
				if (list.Count > 0)
				{
					trEDir = list[Random.Range(0, list.Count)];
				}
				else
				{
					trActive = false;
					trAlive = true;
				}
			}
			else
			{
				trER = num6;
				trEC = num7;
				trGrid[trER, trEC] = 2;
			}
		}
		for (int j = 0; j < trSize; j++)
		{
			for (int k = 0; k < trSize; k++)
			{
				float num11 = num2 + (float)k * num;
				float num12 = num3 + (float)j * num;
				if (trGrid[j, k] == 1)
				{
					GUI.backgroundColor = new Color(0.2f, 0.7f, 1f);
				}
				else if (trGrid[j, k] == 2)
				{
					GUI.backgroundColor = new Color(1f, 0.4f, 0.1f);
				}
				else
				{
					GUI.backgroundColor = new Color(0.1f, 0.1f, 0.12f);
				}
				GUI.Box(new Rect(num11, num12, num - 1f, num - 1f), "");
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 400f, 400f, 20f), "<size=11>WASD or Arrow Keys to steer. Don't crash!</size>");
	}

	private void TRNewGame()
	{
		trGrid = new int[trSize, trSize];
		trPR = trSize / 2;
		trPC = trSize / 4;
		trER = trSize / 2;
		trEC = trSize * 3 / 4;
		trPDir = 1;
		trEDir = 3;
		trGrid[trPR, trPC] = 1;
		trGrid[trER, trEC] = 2;
		trActive = true;
		trAlive = true;
		trScore = 0;
	}

	private void DrawBomberman()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_0926: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c9: Expected O, but got Unknown
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		if (bmGrid == null)
		{
			BMNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Bomberman</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Score: {bmScore}  Lives: {bmLives}  Bombs: {bmBombs}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			BMNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 32f;
		float num2 = 200f;
		float num3 = 80f;
		if (bmActive)
		{
			if (bmBombPlaced)
			{
				bmBombTimer -= Time.deltaTime;
				if (bmBombTimer <= 0f)
				{
					bmBombPlaced = false;
					bmGrid[bmBombR, bmBombC] = 0;
					int[] array = new int[4] { -1, 0, 1, 0 };
					int[] array2 = new int[4] { 0, 1, 0, -1 };
					for (int i = 0; i < 4; i++)
					{
						for (int j = 1; j <= bmRange; j++)
						{
							int num4 = bmBombR + array[i] * j;
							int num5 = bmBombC + array2[i] * j;
							if (num4 < 0 || num4 >= bmSize || num5 < 0 || num5 >= bmSize || bmGrid[num4, num5] == 1)
							{
								break;
							}
							if (bmGrid[num4, num5] == 2)
							{
								bmGrid[num4, num5] = 0;
								bmScore += 10;
							}
							bmGrid[bmBombR, bmBombC] = 0;
						}
					}
					for (int k = 0; k < 4; k++)
					{
						for (int l = 1; l <= bmRange; l++)
						{
							int num6 = bmBombR + array[k] * l;
							int num7 = bmBombC + array2[k] * l;
							if (num6 < 0 || num6 >= bmSize || num7 < 0 || num7 >= bmSize || bmGrid[num6, num7] == 1)
							{
								break;
							}
							bmGrid[num6, num7] = 3;
						}
					}
					bmGrid[bmBombR, bmBombC] = 3;
				}
			}
			if (!bmBombPlaced)
			{
				for (int m = 0; m < bmSize; m++)
				{
					for (int n = 0; n < bmSize; n++)
					{
						if (bmGrid[m, n] == 3)
						{
							bmGrid[m, n] = 0;
						}
					}
				}
			}
			bmEnemyTimer -= Time.deltaTime;
			if (bmEnemyTimer <= 0f && bmEnemies.Count > 0)
			{
				bmEnemyTimer = 0.5f;
				for (int num8 = bmEnemies.Count - 1; num8 >= 0; num8--)
				{
					int[] array3 = new int[4] { 0, 1, 2, 3 };
					int[] array4 = new int[4] { -1, 0, 1, 0 };
					int[] array5 = new int[4] { 0, 1, 0, -1 };
					List<int> list = new List<int>();
					int[] array6 = array3;
					foreach (int num10 in array6)
					{
						int num11 = bmEnemies[num8][0] + array4[num10];
						int num12 = bmEnemies[num8][1] + array5[num10];
						if (num11 >= 0 && num11 < bmSize && num12 >= 0 && num12 < bmSize && (bmGrid[num11, num12] == 0 || bmGrid[num11, num12] == 3))
						{
							list.Add(num10);
						}
					}
					if (list.Count > 0)
					{
						int num13 = list[Random.Range(0, list.Count)];
						bmGrid[bmEnemies[num8][0], bmEnemies[num8][1]] = 0;
						bmEnemies[num8][0] += array4[num13];
						bmEnemies[num8][1] += array5[num13];
						bmGrid[bmEnemies[num8][0], bmEnemies[num8][1]] = 4;
					}
				}
			}
			for (int num14 = bmEnemies.Count - 1; num14 >= 0; num14--)
			{
				if (bmEnemies[num14][0] == bmPR && bmEnemies[num14][1] == bmPC)
				{
					bmLives--;
					if (bmLives <= 0)
					{
						bmActive = false;
					}
					bmPR = 1;
					bmPC = 1;
				}
			}
			if (bmGrid[bmPR, bmPC] == 3)
			{
				bmLives--;
				if (bmLives <= 0)
				{
					bmActive = false;
				}
				bmPR = 1;
				bmPC = 1;
			}
		}
		for (int num15 = 0; num15 < bmSize; num15++)
		{
			for (int num16 = 0; num16 < bmSize; num16++)
			{
				float num17 = num2 + (float)num16 * num;
				float num18 = num3 + (float)num15 * num;
				switch (bmGrid[num15, num16])
				{
				case 1:
					GUI.backgroundColor = new Color(0.4f, 0.3f, 0.2f);
					break;
				case 2:
					GUI.backgroundColor = new Color(0.2f, 0.6f, 0.2f);
					break;
				case 3:
					GUI.backgroundColor = new Color(1f, 0.6f, 0f);
					break;
				case 4:
					GUI.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
					break;
				default:
					GUI.backgroundColor = new Color(0.15f, 0.15f, 0.18f);
					break;
				}
				if (num15 == bmPR && num16 == bmPC)
				{
					GUI.backgroundColor = Color.cyan;
				}
				if (GUI.Button(new Rect(num17, num18, num - 1f, num - 1f), "") && bmActive && bmGrid[num15, num16] == 0 && Mathf.Abs(num15 - bmPR) + Mathf.Abs(num16 - bmPC) == 1)
				{
					bmPR = num15;
					bmPC = num16;
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (!bmActive)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = Color.red;
			GUI.Label(new Rect(200f, num3 + (float)bmSize * num + 10f, 200f, 30f), "Game Over!", val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(170f, 400f, 400f, 20f), "<size=11>Click adjacent tiles to move. Destroy blocks (green), avoid enemies (red)!</size>");
	}

	private void BMNewGame()
	{
		bmGrid = new int[bmSize, bmSize];
		for (int i = 0; i < bmSize; i++)
		{
			for (int j = 0; j < bmSize; j++)
			{
				if (i % 2 == 0 && j % 2 == 0)
				{
					bmGrid[i, j] = 1;
				}
				else if (Random.Range(0, 3) == 0)
				{
					bmGrid[i, j] = 2;
				}
				else
				{
					bmGrid[i, j] = 0;
				}
			}
		}
		bmGrid[1, 1] = 0;
		bmGrid[1, 2] = 0;
		bmGrid[2, 1] = 0;
		bmPR = 1;
		bmPC = 1;
		bmBombs = 1;
		bmRange = 2;
		bmLives = 3;
		bmScore = 0;
		bmActive = true;
		bmBombPlaced = false;
		bmBombTimer = 0f;
		bmEnemyTimer = 0.5f;
		bmEnemies = new List<int[]>();
		for (int k = 0; k < 3; k++)
		{
			int num;
			int num2;
			do
			{
				num = Random.Range(1, bmSize);
				num2 = Random.Range(1, bmSize);
			}
			while (bmGrid[num, num2] != 0);
			bmEnemies.Add(new int[2] { num, num2 });
			bmGrid[num, num2] = 4;
		}
	}

	private void DrawBrickCalculator()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		if (brcGrid == null)
		{
			BRCNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Brick Calculator</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), brcWon ? "You win!" : (brcGameOver ? "Boom!" : $"Bombs: {brcBombs}  Flags: {brcFlagsLeft}"));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			BRCNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		GUI.backgroundColor = (Color)(brcFlagMode ? new Color(0.8f, 0.4f, 0.2f) : guiColorB);
		if (GUI.Button(new Rect(420f, 46f, 75f, 22f), brcFlagMode ? "Flag On" : "Flag Off"))
		{
			brcFlagMode = !brcFlagMode;
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 38f;
		float num2 = 200f;
		float num3 = 80f;
		Color[] array = (Color[])(object)new Color[9]
		{
			Color.white,
			Color.blue,
			Color.green,
			Color.red,
			new Color(0f, 0f, 0.5f),
			new Color(0.5f, 0f, 0f),
			Color.cyan,
			Color.black,
			Color.gray
		};
		for (int i = 0; i < brcSize; i++)
		{
			for (int j = 0; j < brcSize; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				if (brcRevealed[i, j])
				{
					if (brcGrid[i, j] == -1)
					{
						GUI.backgroundColor = new Color(1f, 0.2f, 0.2f);
						GUI.Label(new Rect(num4 + 10f, num5 + 8f, 20f, 20f), "*");
						continue;
					}
					GUI.backgroundColor = new Color(0.85f, 0.85f, 0.8f);
					GUIStyle val = new GUIStyle(GUI.skin.label)
					{
						fontSize = 14,
						fontStyle = (FontStyle)1,
						alignment = (TextAnchor)4
					};
					GUI.color = ((brcGrid[i, j] > 0 && brcGrid[i, j] < array.Length) ? array[brcGrid[i, j]] : Color.black);
					GUI.Label(new Rect(num4, num5, num, num), (brcGrid[i, j] > 0) ? brcGrid[i, j].ToString() : "", val);
					GUI.color = Color.white;
					continue;
				}
				GUI.backgroundColor = (brcFlagged[i, j] ? new Color(0.8f, 0.4f, 0.2f) : new Color(0.4f, 0.4f, 0.45f));
				if (!GUI.Button(new Rect(num4, num5, num - 1f, num - 1f), brcFlagged[i, j] ? "F" : "") || brcGameOver || brcWon)
				{
					continue;
				}
				if (brcFlagMode)
				{
					brcFlagged[i, j] = !brcFlagged[i, j];
					brcFlagsLeft += ((!brcFlagged[i, j]) ? 1 : (-1));
				}
				else
				{
					brcRevealed[i, j] = true;
					if (brcGrid[i, j] == -1)
					{
						brcGameOver = true;
						BRCRevealAll();
					}
					else if (brcGrid[i, j] == 0)
					{
						BRCFloodFill(i, j);
					}
					BRCCheckWin();
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(170f, 400f, 400f, 20f), "<size=11>Click to reveal, toggle Flag mode to mark bombs</size>");
	}

	private void BRCNewGame()
	{
		brcGrid = new int[brcSize, brcSize];
		brcRevealed = new bool[brcSize, brcSize];
		brcFlagged = new bool[brcSize, brcSize];
		brcGameOver = false;
		brcWon = false;
		brcFlagMode = false;
		brcBombs = 10;
		brcFlagsLeft = brcBombs;
		for (int i = 0; i < brcBombs; i++)
		{
			int num;
			int num2;
			do
			{
				num = Random.Range(0, brcSize);
				num2 = Random.Range(0, brcSize);
			}
			while (brcGrid[num, num2] == -1);
			brcGrid[num, num2] = -1;
		}
		for (int j = 0; j < brcSize; j++)
		{
			for (int k = 0; k < brcSize; k++)
			{
				if (brcGrid[j, k] == -1)
				{
					continue;
				}
				int num3 = 0;
				for (int l = -1; l <= 1; l++)
				{
					for (int m = -1; m <= 1; m++)
					{
						int num4 = j + l;
						int num5 = k + m;
						if (num4 >= 0 && num4 < brcSize && num5 >= 0 && num5 < brcSize && brcGrid[num4, num5] == -1)
						{
							num3++;
						}
					}
				}
				brcGrid[j, k] = num3;
			}
		}
	}

	private void BRCFloodFill(int r, int c)
	{
		if (r < 0 || r >= brcSize || c < 0 || c >= brcSize || brcRevealed[r, c] || brcGrid[r, c] == -1)
		{
			return;
		}
		brcRevealed[r, c] = true;
		if (brcGrid[r, c] != 0)
		{
			return;
		}
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				BRCFloodFill(r + i, c + j);
			}
		}
	}

	private void BRCRevealAll()
	{
		for (int i = 0; i < brcSize; i++)
		{
			for (int j = 0; j < brcSize; j++)
			{
				brcRevealed[i, j] = true;
			}
		}
	}

	private void BRCCheckWin()
	{
		for (int i = 0; i < brcSize; i++)
		{
			for (int j = 0; j < brcSize; j++)
			{
				if (!brcRevealed[i, j] && brcGrid[i, j] != -1)
				{
					return;
				}
			}
		}
		brcWon = true;
	}

	private void DrawOthello()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		if (othBoard == null)
		{
			OTHNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Othello (Reversi)</size>");
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (othBoard[i, j] == 1)
				{
					num++;
				}
				else if (othBoard[i, j] == 2)
				{
					num2++;
				}
			}
		}
		GUI.Label(new Rect(170f, 48f, 400f, 20f), othGameOver ? $"Game Over! Black: {num} White: {num2}" : (((othTurn == 0) ? "Black's turn" : "White's turn") + $"  Black: {num} White: {num2}"));
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			OTHNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num3 = 42f;
		float num4 = 210f;
		float num5 = 80f;
		bool flag = false;
		for (int k = 0; k < 8; k++)
		{
			for (int l = 0; l < 8; l++)
			{
				if (othBoard[k, l] == 0 && OTHCanPlace(k, l, othTurn + 1))
				{
					flag = true;
				}
			}
		}
		for (int m = 0; m < 8; m++)
		{
			for (int n = 0; n < 8; n++)
			{
				float num6 = num4 + (float)n * num3;
				float num7 = num5 + (float)m * num3;
				GUI.backgroundColor = new Color(0.1f, 0.5f, 0.1f);
				GUI.Box(new Rect(num6, num7, num3 - 1f, num3 - 1f), "");
				if (othBoard[m, n] != 0)
				{
					GUI.backgroundColor = ((othBoard[m, n] == 1) ? Color.black : Color.white);
					float num8 = num3 * 0.7f;
					GUI.Box(new Rect(num6 + (num3 - num8) / 2f, num7 + (num3 - num8) / 2f, num8, num8), "");
				}
				if (!othGameOver && flag && othTurn == 0 && GUI.Button(new Rect(num6, num7, num3 - 1f, num3 - 1f), "") && othBoard[m, n] == 0 && OTHCanPlace(m, n, 1))
				{
					OTHPlace(m, n, 1);
					othTurn = 1;
					if (!OTHHasMove(2))
					{
						othTurn = 0;
					}
					OTHCheckEnd();
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
			}
		}
		GUI.backgroundColor = guiColorB;
		if (!othGameOver && othTurn == 1)
		{
			((MonoBehaviour)this).Invoke("OTHAIMove", 0.3f);
		}
		GUI.Label(new Rect(170f, 430f, 400f, 20f), "<size=11>Click to place a stone and flip opponent pieces</size>");
	}

	private void OTHNewGame()
	{
		othBoard = new int[8, 8];
		othBoard[3, 3] = 2;
		othBoard[3, 4] = 1;
		othBoard[4, 3] = 1;
		othBoard[4, 4] = 2;
		othTurn = 0;
		othGameOver = false;
	}

	private bool OTHCanPlace(int r, int c, int p)
	{
		if (othBoard[r, c] != 0)
		{
			return false;
		}
		int[] array = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };
		int[] array2 = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
		for (int i = 0; i < 8; i++)
		{
			int num = r + array[i];
			int num2 = c + array2[i];
			int num3 = 0;
			while (num >= 0 && num < 8 && num2 >= 0 && num2 < 8 && othBoard[num, num2] != 0 && othBoard[num, num2] != p)
			{
				num3++;
				num += array[i];
				num2 += array2[i];
			}
			if (num3 > 0 && num >= 0 && num < 8 && num2 >= 0 && num2 < 8 && othBoard[num, num2] == p)
			{
				return true;
			}
		}
		return false;
	}

	private void OTHPlace(int r, int c, int p)
	{
		othBoard[r, c] = p;
		int[] array = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };
		int[] array2 = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
		for (int i = 0; i < 8; i++)
		{
			int num = r + array[i];
			int num2 = c + array2[i];
			int num3 = 0;
			List<int[]> list = new List<int[]>();
			while (num >= 0 && num < 8 && num2 >= 0 && num2 < 8 && othBoard[num, num2] != 0 && othBoard[num, num2] != p)
			{
				list.Add(new int[2] { num, num2 });
				num += array[i];
				num2 += array2[i];
				num3++;
			}
			if (num3 <= 0 || num < 0 || num >= 8 || num2 < 0 || num2 >= 8 || othBoard[num, num2] != p)
			{
				continue;
			}
			foreach (int[] item in list)
			{
				othBoard[item[0], item[1]] = p;
			}
		}
	}

	private bool OTHHasMove(int p)
	{
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (OTHCanPlace(i, j, p))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OTHCheckEnd()
	{
		if (!OTHHasMove(1) && !OTHHasMove(2))
		{
			othGameOver = true;
		}
	}

	private void OTHAIMove()
	{
		if (othGameOver)
		{
			return;
		}
		int num = -1;
		int c = -1;
		int num2 = -1;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (othBoard[i, j] != 0 || !OTHCanPlace(i, j, 2))
				{
					continue;
				}
				int num3 = 0;
				int[] array = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };
				int[] array2 = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
				for (int k = 0; k < 8; k++)
				{
					int num4 = i + array[k];
					int num5 = j + array2[k];
					while (num4 >= 0 && num4 < 8 && num5 >= 0 && num5 < 8 && othBoard[num4, num5] != 0 && othBoard[num4, num5] != 2)
					{
						num3++;
						num4 += array[k];
						num5 += array2[k];
					}
				}
				if (i == 0 && (j == 0 || j == 7))
				{
					num3 += 50;
				}
				else if (i == 7 && (j == 0 || j == 7))
				{
					num3 += 50;
				}
				else if (i == 0 || i == 7 || j == 0 || j == 7)
				{
					num3 += 10;
				}
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
					c = j;
				}
			}
		}
		if (num >= 0)
		{
			OTHPlace(num, c, 2);
			othTurn = 0;
			if (!OTHHasMove(1))
			{
				othTurn = 1;
			}
			OTHCheckEnd();
		}
		else
		{
			othTurn = 0;
			OTHCheckEnd();
		}
	}

	private void DrawRushHour()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Expected O, but got Unknown
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		if (rhGrid == null)
		{
			RHNewGame();
		}
		GUI.Label(new Rect(170f, 25f, 300f, 30f), "<size=18>Rush Hour</size>");
		GUI.Label(new Rect(170f, 48f, 300f, 20f), $"Moves: {rhMoves}");
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(500f, 46f, 80f, 22f), "New Game"))
		{
			RHNewGame();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		GUI.backgroundColor = guiColorB;
		float num = 50f;
		float num2 = 210f;
		float num3 = 85f;
		Color[] array = (Color[])(object)new Color[7]
		{
			Color.red,
			Color.blue,
			new Color(0.2f, 0.7f, 0.2f),
			new Color(0.8f, 0.8f, 0.2f),
			new Color(0.8f, 0.3f, 0.8f),
			new Color(0.2f, 0.8f, 0.8f),
			new Color(0.9f, 0.5f, 0.2f)
		};
		for (int i = 0; i < rhSize; i++)
		{
			for (int j = 0; j < rhSize; j++)
			{
				float num4 = num2 + (float)j * num;
				float num5 = num3 + (float)i * num;
				if (rhGrid[i, j] == -1)
				{
					GUI.backgroundColor = new Color(0.1f, 0.7f, 0.1f, 0.5f);
				}
				else if (rhGrid[i, j] > 0)
				{
					GUI.backgroundColor = array[(rhGrid[i, j] - 1) % array.Length];
				}
				else
				{
					GUI.backgroundColor = new Color(0.25f, 0.25f, 0.3f);
				}
				if (!GUI.Button(new Rect(num4, num5, num - 2f, num - 2f), (rhGrid[i, j] == 1) ? ">>" : ""))
				{
					continue;
				}
				if (rhSelected < 0)
				{
					rhSelected = rhGrid[i, j];
				}
				else if (rhSelected == rhGrid[i, j])
				{
					rhSelected = -1;
				}
				else
				{
					if (rhCarDir[rhSelected - 1] == 0)
					{
						if (i == rhCarR[rhSelected - 1] && Mathf.Abs(j - rhCarC[rhSelected - 1]) == 1)
						{
							if (j < rhCarC[rhSelected - 1])
							{
								RHMoveCar(rhSelected, -1);
							}
							else
							{
								RHMoveCar(rhSelected, 1);
							}
						}
					}
					else if (j == rhCarC[rhSelected - 1] && Mathf.Abs(i - rhCarR[rhSelected - 1]) == 1)
					{
						if (i < rhCarR[rhSelected - 1])
						{
							RHMoveCar(rhSelected, -1);
						}
						else
						{
							RHMoveCar(rhSelected, 1);
						}
					}
					rhSelected = -1;
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			}
		}
		GUI.backgroundColor = guiColorB;
		if (rhSelected > 0)
		{
			GUI.Label(new Rect(200f, num3 + (float)rhSize * num + 10f, 300f, 20f), $"<size=11>Car {rhSelected} selected. Click an adjacent empty cell to move.</size>");
		}
		if (rhSolved)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			GUI.color = Color.green;
			GUI.Label(new Rect(200f, num3 + (float)rhSize * num + 35f, 200f, 30f), $"Solved in {rhMoves} moves!", val);
			GUI.color = Color.white;
		}
		GUI.Label(new Rect(170f, 420f, 400f, 20f), "<size=11>Click a car, then click adjacent empty cell to slide it. Get the red car to the exit!</size>");
	}

	private void RHNewGame()
	{
		rhGrid = new int[rhSize, rhSize];
		rhCarR = new int[6];
		rhCarC = new int[6];
		rhCarLen = new int[6];
		rhCarDir = new int[6];
		rhMoves = 0;
		rhSolved = false;
		rhSelected = -1;
		for (int i = 0; i < 6; i++)
		{
			rhCarDir[i] = ((i >= 2) ? 1 : 0);
			rhCarLen[i] = ((i == 0) ? 3 : Random.Range(2, 4));
		}
		rhCarR[0] = 2;
		rhCarC[0] = 0;
		rhCarLen[0] = 3;
		rhCarDir[0] = 0;
		for (int j = 1; j < 6; j++)
		{
			bool flag = false;
			int num = 0;
			while (!flag && num < 100)
			{
				num++;
				rhCarR[j] = Random.Range(0, rhSize);
				rhCarC[j] = Random.Range(0, rhSize);
				rhCarLen[j] = Random.Range(2, 4);
				bool flag2 = true;
				for (int k = 0; k < rhCarLen[j]; k++)
				{
					int num2 = ((rhCarDir[j] == 0) ? rhCarR[j] : (rhCarR[j] + k));
					int num3 = ((rhCarDir[j] == 0) ? (rhCarC[j] + k) : rhCarC[j]);
					if (num2 < 0 || num2 >= rhSize || num3 < 0 || num3 >= rhSize || rhGrid[num2, num3] != 0)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					for (int l = 0; l < rhCarLen[j]; l++)
					{
						int num4 = ((rhCarDir[j] == 0) ? rhCarR[j] : (rhCarR[j] + l));
						int num5 = ((rhCarDir[j] == 0) ? (rhCarC[j] + l) : rhCarC[j]);
						rhGrid[num4, num5] = j + 1;
					}
					flag = true;
				}
			}
		}
		rhGrid[2, 5] = -1;
	}

	private void RHMoveCar(int car, int delta)
	{
		int num = rhCarDir[car - 1];
		int num2 = rhCarLen[car - 1];
		int num3 = rhCarR[car - 1] + ((num == 1) ? delta : 0);
		int num4 = rhCarC[car - 1] + ((num == 0) ? delta : 0);
		int num5 = ((num == 0) ? num3 : (num3 + num2 - 1));
		int num6 = ((num == 0) ? (num4 + num2 - 1) : num4);
		if (num3 < 0 || num3 >= rhSize || num4 < 0 || num4 >= rhSize || num5 < 0 || num5 >= rhSize || num6 < 0 || num6 >= rhSize)
		{
			return;
		}
		for (int i = 0; i < num2; i++)
		{
			int num7 = ((num == 0) ? num3 : (num3 + i));
			int num8 = ((num == 0) ? (num4 + i) : num4);
			if (rhGrid[num7, num8] != 0 && rhGrid[num7, num8] != car)
			{
				return;
			}
		}
		for (int j = 0; j < num2; j++)
		{
			int num9 = ((num == 0) ? rhCarR[car - 1] : (rhCarR[car - 1] + j));
			int num10 = ((num == 0) ? (rhCarC[car - 1] + j) : rhCarC[car - 1]);
			rhGrid[num9, num10] = 0;
		}
		rhCarR[car - 1] = num3;
		rhCarC[car - 1] = num4;
		for (int k = 0; k < num2; k++)
		{
			int num11 = ((num == 0) ? num3 : (num3 + k));
			int num12 = ((num == 0) ? (num4 + k) : num4);
			rhGrid[num11, num12] = car;
		}
		rhMoves++;
		if (rhCarR[0] == 2 && rhCarC[0] + rhCarLen[0] - 1 >= rhSize - 1)
		{
			rhSolved = true;
		}
	}

	private void DrawSupportTab()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0998: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
		float num = 510f;
		float num2 = 1200f;
		reportListScroll = GUI.BeginScrollView(new Rect(170f, 21f, 530f, 395f), reportListScroll, new Rect(0f, 0f, num, num2), false, true);
		float num3 = 4f;
		float num4 = 6f;
		GUI.backgroundColor = guiColorA;
		GUI.Label(new Rect(num4, num3, num, 22f), "<b>Report a Broken Mod</b>");
		num3 += 24f;
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num4, num3, num, 18f), "Mod Name:");
		num3 += 18f;
		reportModName = GUI.TextField(new Rect(num4, num3, num, 22f), reportModName);
		num3 += 28f;
		GUI.Label(new Rect(num4, num3, num, 18f), "Description of the issue:");
		num3 += 18f;
		GUI.backgroundColor = new Color(0.12f, 0.12f, 0.17f, 0.95f);
		reportDescription = GUI.TextArea(new Rect(num4, num3, num, 80f), reportDescription);
		num3 += 86f;
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num4, num3, num, 18f), "Screenshot URL (optional):");
		num3 += 18f;
		reportScreenshotUrl = GUI.TextField(new Rect(num4, num3, num, 22f), reportScreenshotUrl);
		num3 += 28f;
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(num4, num3, 150f, 25f), "Submit Report"))
		{
			if (string.IsNullOrWhiteSpace(reportModName) || string.IsNullOrWhiteSpace(reportDescription))
			{
				reportStatus = "<color=red>Please fill in the mod name and description.</color>";
				reportStatusTimer = Time.time;
			}
			else
			{
				SubmitSupportReport(reportModName, reportDescription, reportScreenshotUrl);
				reportModName = "";
				reportDescription = "";
				reportScreenshotUrl = "";
				reportStatus = "<color=green>Report submitted! The owner will be notified.</color>";
				reportStatusTimer = Time.time;
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num3 += 30f;
		if (!string.IsNullOrEmpty(reportStatus) && Time.time - reportStatusTimer < 5f)
		{
			Color contentColor = GUI.contentColor;
			GUI.contentColor = Color.white;
			GUI.Label(new Rect(num4, num3, num, 20f), reportStatus);
			GUI.contentColor = contentColor;
			num3 += 22f;
		}
		num3 += 10f;
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num4, num3, num, 20f), "<b>Submitted Reports</b>");
		num3 += 22f;
		if (reportEntries.Count == 0)
		{
			GUI.Label(new Rect(num4 + 10f, num3, num - 10f, 18f), "<color=#666666>No reports submitted yet.</color>");
			num3 += 20f;
		}
		else
		{
			for (int num5 = reportEntries.Count - 1; num5 >= 0; num5--)
			{
				SupportReportEntry supportReportEntry = reportEntries[num5];
				GUI.backgroundColor = new Color(0.12f, 0.12f, 0.17f, 0.95f);
				GUI.Box(new Rect(num4, num3, num, 70f), "");
				GUI.backgroundColor = guiColorA;
				GUI.Label(new Rect(num4 + 6f, num3 + 2f, num - 12f, 18f), "<b>" + supportReportEntry.modName + "</b>  <color=#888888>by " + supportReportEntry.sender + "</color>");
				GUI.backgroundColor = guiColorB;
				string text = ((supportReportEntry.description.Length > 120) ? (supportReportEntry.description.Substring(0, 117) + "...") : supportReportEntry.description);
				GUI.Label(new Rect(num4 + 6f, num3 + 20f, num - 12f, 30f), text);
				GUI.Label(new Rect(num4 + 6f, num3 + 50f, num - 12f, 16f), "<color=#666666>" + supportReportEntry.timestamp + "</color>");
				if (!string.IsNullOrEmpty(supportReportEntry.screenshotUrl) && GUI.Button(new Rect(num4 + num - 90f, num3 + 48f, 84f, 18f), "Screenshot"))
				{
					Application.OpenURL(supportReportEntry.screenshotUrl);
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				}
				num3 += 76f;
			}
		}
		num3 += 10f;
		GUI.backgroundColor = guiColorA;
		GUI.Label(new Rect(num4, num3, num, 20f), "<b>Player Role Lookup</b>");
		num3 += 22f;
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num4, num3, num, 18f), "Type a player name or User ID to check their role:");
		num3 += 20f;
		GUI.backgroundColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
		GUI.SetNextControlName("SupportLookup");
		supportLookupInput = GUI.TextField(new Rect(num4, num3, 370f, 22f), supportLookupInput);
		GUI.backgroundColor = guiColorA;
		if (GUI.Button(new Rect(num4 + 380f, num3, 120f, 22f), "Lookup"))
		{
			supportLookupResult = "";
			string text2 = supportLookupInput.Trim();
			if (string.IsNullOrEmpty(text2))
			{
				supportLookupResult = "Enter a name or User ID.";
				supportLookupColor = Color.yellow;
			}
			else
			{
				bool flag = false;
				Player[] playerList = PhotonNetwork.PlayerList;
				for (int i = 0; i < playerList.Length; i++)
				{
					NetPlayer val = NetPlayer.op_Implicit(playerList[i]);
					if (val.UserId == text2)
					{
						text2 = val.NickName;
						flag = true;
						break;
					}
				}
				string text3 = null;
				string text4 = null;
				foreach (KeyValuePair<string, string> administrator in ServerData.Administrators)
				{
					if (administrator.Key == text2 || administrator.Value.Equals(text2, StringComparison.OrdinalIgnoreCase))
					{
						text4 = administrator.Key;
						text3 = administrator.Value;
						break;
					}
				}
				if (text3 == null && flag)
				{
					foreach (KeyValuePair<string, string> administrator2 in ServerData.Administrators)
					{
						if (administrator2.Key == text2)
						{
							text4 = administrator2.Key;
							text3 = administrator2.Value;
							break;
						}
					}
				}
				if (text3 != null)
				{
					if (ServerData.OwnerUserIds.Contains(text4))
					{
						supportLookupResult = text3 + " (" + text4 + ") is an OWNER.";
						supportLookupColor = new Color(1f, 0.84f, 0f);
					}
					else if (ServerData.SuperAdministrators.Contains(text3))
					{
						supportLookupResult = text3 + " (" + text4 + ") is a SUPER-ADMIN.";
						supportLookupColor = new Color(0.8f, 0.2f, 1f);
					}
					else
					{
						supportLookupResult = text3 + " (" + text4 + ") is an ADMIN.";
						supportLookupColor = new Color(0.3f, 0.9f, 0.4f);
					}
				}
				else
				{
					supportLookupResult = text2 + " is not an admin/owner.";
					supportLookupColor = new Color(0.6f, 0.6f, 0.6f);
				}
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
		num3 += 28f;
		if (!string.IsNullOrEmpty(supportLookupResult))
		{
			Color contentColor2 = GUI.contentColor;
			GUI.contentColor = supportLookupColor;
			GUI.Label(new Rect(num4, num3, num, 20f), supportLookupResult);
			GUI.contentColor = contentColor2;
			num3 += 24f;
		}
		num3 += 10f;
		GUI.backgroundColor = guiColorB;
		GUI.Label(new Rect(num4, num3, num, 20f), "<b>All Owners</b>");
		num3 += 20f;
		foreach (string ownerUserId in ServerData.OwnerUserIds)
		{
			string text5 = (ServerData.Administrators.ContainsKey(ownerUserId) ? ServerData.Administrators[ownerUserId] : "(unknown)");
			GUI.Label(new Rect(num4 + 10f, num3, num - 10f, 16f), "<color=#FFD700>" + text5 + "</color>  <color=#888888>" + ownerUserId + "</color>");
			num3 += 16f;
		}
		num3 += 6f;
		GUI.Label(new Rect(num4, num3, num, 20f), "<b>All Super-Admins</b>");
		num3 += 20f;
		foreach (string superAdministrator in ServerData.SuperAdministrators)
		{
			GUI.Label(new Rect(num4 + 10f, num3, num - 10f, 16f), "<color=#CC33FF>" + superAdministrator + "</color>");
			num3 += 16f;
		}
		num3 += 6f;
		GUI.Label(new Rect(num4, num3, num, 20f), "<b>All Admins</b>");
		num3 += 20f;
		foreach (KeyValuePair<string, string> administrator3 in ServerData.Administrators)
		{
			string key = administrator3.Key;
			string value = administrator3.Value;
			bool flag2 = ServerData.OwnerUserIds.Contains(key);
			bool flag3 = ServerData.SuperAdministrators.Contains(value);
			if (flag2)
			{
				GUI.Label(new Rect(num4 + 10f, num3, num - 10f, 16f), "<color=#FFD700>" + value + "</color>  <color=#888888>" + key + "</color>  <color=#FFD700>[Owner]</color>");
			}
			else if (flag3)
			{
				GUI.Label(new Rect(num4 + 10f, num3, num - 10f, 16f), "<color=#CC33FF>" + value + "</color>  <color=#888888>" + key + "</color>  <color=#CC33FF>[Super-Admin]</color>");
			}
			else
			{
				GUI.Label(new Rect(num4 + 10f, num3, num - 10f, 16f), "<color=#4DE64D>" + value + "</color>  <color=#888888>" + key + "</color>");
			}
			num3 += 16f;
		}
		GUI.EndScrollView();
	}

	private async void SubmitSupportReport(string modName, string description, string screenshotUrl)
	{
		try
		{
			string nick = (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? "Unknown" : PhotonNetwork.LocalPlayer.NickName);
			string uid = PhotonNetwork.LocalPlayer.UserId ?? "unknown";
			string timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
			reportEntries.Add(new SupportReportEntry
			{
				sender = nick,
				modName = modName,
				description = description,
				screenshotUrl = (screenshotUrl ?? ""),
				timestamp = timestamp
			});
			string webhook = "https://discord.com/api/webhooks/1523079492975853679/IW5B1EshhbhK42hqkW2jOUjLQTLE96L7DI1QP5zZQPGn2m__X2DL1bb1IRkKpO1pXdMY";
			string content = "**New Support Report**\n**Mod:** " + modName + "\n**Issue:** " + description + "\n**From:** " + nick + " (`" + uid + "`)\n**Time:** " + timestamp;
			if (!string.IsNullOrEmpty(screenshotUrl))
			{
				content = content + "\n**Screenshot:** " + screenshotUrl;
			}
			using HttpClient client = new HttpClient();
			StringContent payload = new StringContent("{\"content\":\"" + content.Replace("\n", "\\n").Replace("\"", "\\\"") + "\"}", Encoding.UTF8, "application/json");
			await client.PostAsync(webhook, payload);
		}
		catch
		{
		}
	}
}
