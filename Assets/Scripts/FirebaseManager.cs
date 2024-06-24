using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Mkey;
using System;
using System.Linq;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;

    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }

    public string playerUserId;

    public List <Pair> coinList = new List<Pair>();
    public List <Pair> mostCoinsTodayList = new List<Pair>();
    public List <Pair> mostCoinsYesterdayList = new List<Pair>();

    public List<Pair> levelList = new List<Pair>();
    public List<Pair> levelYesterdayList = new List<Pair>();
    public List<Pair> levelTodayList = new List<Pair>();

    public List<Pair> coinsTodayList = new List<Pair>();
    public List<Pair> coinsTodayYesterdayList = new List<Pair>();

    public int playerMostCoinsRank;
    public int playerMostCoinsTodayRank;
    public int playerMostCoinsYesterdayRank;

    public int playerHighestLevelRank;
    public int playerHighestLevelYesterdayRank;
    public int playerHighestLevelTodayRank;

    public int playerCoinsTodayRank;
    public int playerCoinsTodayYesterdayRank;

    private int playerCoinsToday;
    private long playerCoinsYesterday;
    private long playerLevelYesterday;
    private long playerMostCoinsYesterday;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        playerUserId = SystemInfo.deviceUniqueIdentifier;
        DBreference = FirebaseDatabase.GetInstance("https://slots-world-tour-2024-default-rtdb.firebaseio.com").RootReference;

        StartCoroutine(UpdateCoinsData(MPlayer.Coins));
        StartCoroutine(UpdateLevelData(MPlayer.Level));

        if (PlayerPrefs.GetInt("FirstDayPassed", 0) == 0) 
            StartCoroutine(CheckIfTimeAndDayEntryExists());

        StartCoroutine(CheckIfTodayHasEnded());

        StartCoroutine(GetCoinsDataFromFirebase());
        StartCoroutine(GetMostCoinsTodayDataFromFirebase());
        StartCoroutine(GetMostCoinsYesterdayDataFromFirebase());

        StartCoroutine(GetLevelDataFromFirebase());
        StartCoroutine(GetYesterdayLevelDataFromFirebase());
        StartCoroutine(GetTodayLevelDataFromFirebase());

        StartCoroutine(GetCoinsTodayDataFromFirebase());
        StartCoroutine(GetMostCoinsTodayYesterdayDataFromFirebase());
    }

    public int GetPlayerLevel()
    {
        return MPlayer.Level;
    }

    public int GetPlayerCoins()
    {
        return MPlayer.Coins;
    }

    public int GetPlayerCoinsToday()
    {
        return playerCoinsToday;
    }

    public long GetPlayerLevelYesterday()
    {
        return playerLevelYesterday;
    }

    public long GetPlayerMostCoinsYesterday()
    {
        return playerMostCoinsYesterday;
    }

    public long GetPlayerCoinsYesterday()
    {
        return playerCoinsYesterday;
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase");
        playerUserId = SystemInfo.deviceUniqueIdentifier;
        DBreference = FirebaseDatabase.GetInstance("https://slots-world-tour-2024-default-rtdb.firebaseio.com").RootReference;
    }

    private IEnumerator UpdateCoinsData(int coins)
    {
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("Overall").Child("CurrentCoins").SetValueAsync(coins);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // updated
        }

        var DBTaskCoinsToday = DBreference.Child("users").Child(playerUserId).Child("Today").Child("CurrentCoins").SetValueAsync(coins);

        yield return new WaitUntil(predicate: () => DBTaskCoinsToday.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // updated
        }
    }

    public void UpdateTodayCoins(int coins)
    {
        StartCoroutine(UpdateTodayCoinsData(coins));
    }
    public IEnumerator UpdateTodayCoinsData(int coins)
    {
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("Today").Child("MostCoinsToday").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);


        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Get the current coins value
                int currentCoins = Convert.ToInt32(snapshot.Value);

                // Calculate the new coins value by adding the coins to add
                int newCoins = currentCoins + coins;

                // Update the coins value in the database
                DBreference.Child("users").Child(playerUserId).Child("Today").Child("MostCoinsToday").SetValueAsync(newCoins);
            }
        });
    }

    public void ResetTodayCoins()
    {
        StartCoroutine(ResetTodayCoinsData());
    }

    public IEnumerator ResetTodayCoinsData()
    {
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("Today").Child("MostCoinsToday").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);


        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Update the coins value in the database
                DBreference.Child("users").Child(playerUserId).Child("Yesterday").Child("MostCoinsToday").SetValueAsync(Convert.ToInt32(snapshot.Value));
                DBreference.Child("users").Child(playerUserId).Child("Today").Child("MostCoinsToday").SetValueAsync(0);
            }
        });

        var DBTaskLevel = DBreference.Child("users").Child(playerUserId).Child("Today").Child("CurrentLevel").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTaskLevel.IsCompleted);


        DBTaskLevel.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Update the coins value in the database
                DBreference.Child("users").Child(playerUserId).Child("Yesterday").Child("CurrentLevel").SetValueAsync(Convert.ToInt32(snapshot.Value));
            }
        });

        var DBTaskCoinsOverall = DBreference.Child("users").Child(playerUserId).Child("Today").Child("CurrentCoins").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTaskCoinsOverall.IsCompleted);


        DBTaskCoinsOverall.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Update the coins value in the database
                DBreference.Child("users").Child(playerUserId).Child("Yesterday").Child("CurrentCoins").SetValueAsync(Convert.ToInt32(snapshot.Value));
            }
        });
    }

    private IEnumerator UpdateLevelData(int level)
    {
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("Overall").Child("CurrentLevel").SetValueAsync(level);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // updated
        }

        var DBTaskTodayLevel = DBreference.Child("users").Child(playerUserId).Child("Today").Child("CurrentLevel").SetValueAsync(level);

        yield return new WaitUntil(predicate: () => DBTaskTodayLevel.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // updated
        }
    }

    private IEnumerator CheckIfTodayHasEnded()
    {
        // Reference to the node you want to check for existence
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("Today").Child("Time").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data asynchronously
        DBTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error
                Debug.LogError("Error checking for entry existence: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    string jsonData = snapshot.GetRawJsonValue();

                    // Deserialize the JSON into your custom data structure
                    TimeAndDayData timeAndDayData = JsonUtility.FromJson<TimeAndDayData>(jsonData);

                    double timeDifference = CalculateTimeAndDayDifference(timeAndDayData.time, timeAndDayData.day);

                    if (Math.Abs(timeDifference) >= 24f)
                    {
                        ResetTodayCoins();
                        WriteTodayTimeToDatabase();
                        PlayerPrefs.SetInt("TodayPassed", 1);
                    }
                }
                else
                {
                    Debug.Log("Today Time entry does not exist.");
                    PlayerPrefs.SetInt("TodayPassed", 0);
                    WriteTodayTimeToDatabase();
                }
            }
        });
    }

    private async void WriteTodayTimeToDatabase()
    {
        // Get the current time and day
        string currentTime = DateTime.Now.ToString("HH:mm:ss");
        string currentDay = DateTime.Now.DayOfWeek.ToString();

        // Create a data object to store time and day
        TimeAndDayData timeAndDayData = new TimeAndDayData(currentTime, currentDay);

        // Serialize the data object into JSON
        string jsonData = JsonUtility.ToJson(timeAndDayData);

        // Write the serialized data to the Firebase database
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("Today").Child("Time").SetRawJsonValueAsync(jsonData);
        await DBTask;

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // updated
        }
    }
    private IEnumerator GetCoinsTodayDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Today/MostCoinsToday").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);


        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                int count = 1;

                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    // Access the coinsOverall value for each user
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long coinsToday = (long)userSnapshot.Child("Today").Child("MostCoinsToday").Value;
                    coinsTodayList.Add(new Pair(userId, coinsToday.ToString()));
                    if (playerUserId == userId)
                    {
                        playerCoinsTodayRank = count;
                        playerCoinsToday = (int) coinsToday;
                    }
                    else
                        count++;
                    //print(coinsOverall);
                }
                //foreach (DataSnapshot userSnapShot in snapshot.Children)
                //{
                //    string userId = userSnapShot.Key; // Get the key which is the user ID

                //    Dictionary<string, object> userData = (Dictionary<string, object>)userSnapShot.Child("Today").Value;
                    
                //    int coins = Convert.ToInt32(userData["MostCoinsToday"]);

                //    coinsTodayList.Add(new Pair(userId, coins.ToString()));

                //    if (playerUserId == userId)
                //    {
                //        playerCoinsTodayRank = count;
                //        playerCoinsToday = coins;
                //    }
                //    else
                //        count++;
                //}
            }
        });
    }

    private IEnumerator GetCoinsDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Overall/CurrentCoins").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);


        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int count = 1;

                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    // Access the coinsOverall value for each user
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long coinsOverall = (long)userSnapshot.Child("Overall").Child("CurrentCoins").Value;
                    coinList.Add(new Pair(userId, coinsOverall.ToString()));
                    if (playerUserId == userId)
                        playerMostCoinsRank = count;
                    else
                        count++;
                    //print(coinsOverall);
                }
            }
        });
    }

    private IEnumerator GetLevelDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Overall/CurrentLevel").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data once
        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                int count = 1;

                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    // Access the coinsOverall value for each user
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long coinsToday = (long)userSnapshot.Child("Overall").Child("CurrentLevel").Value;
                    levelList.Add(new Pair(userId, coinsToday.ToString()));
                    if (playerUserId == userId)
                    {
                        playerHighestLevelRank = count;
                    }
                    else
                        count++;
                    //print(coinsOverall);
                }
            }
        });
    }

    private IEnumerator GetYesterdayLevelDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Yesterday/CurrentLevel").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data once
        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //print("snapshot"+ snapshot.ChildrenCount);
                int count = 1;

                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long levelYesterday = (long)userSnapshot.Child("Yesterday").Child("CurrentLevel").Value;

                    //print("YESTERDAY" + levelYesterday);
                    levelYesterdayList.Add(new Pair(userId, levelYesterday.ToString()));
                    if (playerUserId == userId)
                    {
                        playerHighestLevelYesterdayRank = count;
                        playerLevelYesterday = levelYesterday;
                    }
                    else
                        count++;
                }
            }
        });
    }

    private IEnumerator GetTodayLevelDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Overall/CurrentLevel").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data once
        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //print("snapshot" + snapshot.ChildrenCount);
                int count = 1;

                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long levelYesterday = (long)userSnapshot.Child("Overall").Child("CurrentLevel").Value;

                    //print("YESTERDAY" + levelYesterday);
                    levelTodayList.Add(new Pair(userId, levelYesterday.ToString()));
                    if (playerUserId == userId)
                    {
                        playerHighestLevelTodayRank = count;
                    }
                    else
                        count++;
                }
            }
        });
    }

    private IEnumerator GetMostCoinsTodayDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Today/CurrentCoins").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data once
        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //print("snapshot" + snapshot.ChildrenCount);
                int count = 1;

                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long levelYesterday = (long)userSnapshot.Child("Today").Child("CurrentCoins").Value;

                    //print("YESTERDAY" + levelYesterday);
                    mostCoinsTodayList.Add(new Pair(userId, levelYesterday.ToString()));
                    if (playerUserId == userId)
                    {
                        playerMostCoinsTodayRank = count;
                    }
                    else
                        count++;
                }
            }
        });
    }

    private IEnumerator GetMostCoinsYesterdayDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Yesterday/CurrentCoins").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data once
        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //print("snapshot" + snapshot.ChildrenCount);
                int count = 1;

                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long levelYesterday = (long)userSnapshot.Child("Yesterday").Child("CurrentCoins").Value;

                    //print("YESTERDAY" + levelYesterday);
                    mostCoinsYesterdayList.Add(new Pair(userId, levelYesterday.ToString()));
                    if (playerUserId == userId)
                    {
                        playerMostCoinsYesterdayRank = count;
                        playerMostCoinsYesterday = levelYesterday;
                    }
                    else
                        count++;
                }
            }
        });
    }
    private IEnumerator GetMostCoinsTodayYesterdayDataFromFirebase()
    {
        var DBTask = DBreference.Child("users").OrderByChild("Yesterday/MostCoinsToday").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data once
        DBTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //print("snapshot" + snapshot.ChildrenCount);
                int count = 1;

                foreach (DataSnapshot userSnapshot in snapshot.Children.Reverse())
                {
                    string userId = userSnapshot.Key; // Get the key which is the user ID

                    long levelYesterday = (long)userSnapshot.Child("Yesterday").Child("MostCoinsToday").Value;

                    //print("YESTERDAY" + levelYesterday);
                    coinsTodayYesterdayList.Add(new Pair(userId, levelYesterday.ToString()));
                    if (playerUserId == userId)
                    {
                        playerCoinsTodayYesterdayRank = count;
                        playerCoinsYesterday = levelYesterday;
                    }
                    else
                        count++;
                }
            }
        });
    }

    private async void WriteTimeAndDayToDatabase()
    {
        // Get the current time and day
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        string currentDay = System.DateTime.Now.DayOfWeek.ToString();

        // Create a data object to store time and day
        TimeAndDayData timeAndDayData = new TimeAndDayData(currentTime, currentDay);

        // Serialize the data object into JSON
        string jsonData = JsonUtility.ToJson(timeAndDayData);

        // Write the serialized data to the Firebase database
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("timeAndDay").SetRawJsonValueAsync(jsonData);
        await DBTask;

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // updated
        }
    }

    public async void GetStartTimeAndDayFromFirebase()
    {
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("timeAndDay").GetValueAsync();
        await DBTask;
        // Fetch the data once
        await DBTask.ContinueWithOnMainThread(task =>
          {
              if (task.IsFaulted)
              {
                // Handle the error...
            }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;

                  string jsonData = snapshot.GetRawJsonValue();

                  // Deserialize the JSON into your custom data structure
                  TimeAndDayData timeAndDayData = JsonUtility.FromJson<TimeAndDayData>(jsonData);

                  // Now you can use timeAndDayData.time and timeAndDayData.day as needed
                  //Debug.Log("Time: " + timeAndDayData.time);
                  //Debug.Log("Day: " + timeAndDayData.day);

                  double timeDifference = CalculateTimeAndDayDifference(timeAndDayData.time, timeAndDayData.day);

                  if (Math.Abs(timeDifference) >= 24)
                  {
                      print("player prefs 24h");
                      PlayerPrefs.SetInt("FirstDayPassed", 1);
                      print("player prefs 24h saved");
                  }

                  //Debug.Log("Time difference in hours: " + timeDifference);
              }
          });
    }

    private IEnumerator CheckIfTimeAndDayEntryExists()
    {
        // Reference to the node you want to check for existence
        var DBTask = DBreference.Child("users").Child(playerUserId).Child("timeAndDay").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        // Fetch the data asynchronously
        DBTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error
                Debug.LogError("Error checking for entry existence: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    //Debug.Log("Entry exists!");
                    GetStartTimeAndDayFromFirebase();
                }
                else
                {
                    //Debug.Log("Entry does not exist.");
                    PlayerPrefs.SetInt("FirstDayPassed", 0);
                    WriteTimeAndDayToDatabase();
                }
            }
        });
    }

    double CalculateTimeAndDayDifference(string retrievedTime, string retrievedDay)
    {
        //Debug.Log("Calculate TIme Difference");
        // Get current time and day
        string currentTime = DateTime.Now.ToString("HH:mm:ss");
        //string currentDay = DateTime.Now.DayOfWeek.ToString();

        // Convert time strings to DateTime objects for easier comparison
        DateTime retrievedDateTime = DateTime.Parse(retrievedTime);
        DateTime currentDateTime = DateTime.Parse(currentTime);

        // Convert day strings to numeric representation for easier comparison
        int retrievedDayIndex = (int)Enum.Parse(typeof(DayOfWeek), retrievedDay);
        int currentDayIndex = (int)DateTime.Now.DayOfWeek;

        // Calculate the day difference
        int dayDifference = currentDayIndex - retrievedDayIndex;

        // Calculate the time difference in seconds
        double timeDifferenceInHours = (dayDifference * 24) + (currentDateTime - retrievedDateTime).TotalHours;

        return timeDifferenceInHours;
    }
}

public class Pair
{
    public string id;
    public string value;

    public Pair(string id, string value)
    {
        this.id = id;
        this.value = value;
    }
}

public class TimeAndDayData
{
    public string time;
    public string day;

    public TimeAndDayData(string time, string day)
    {
        this.time = time;
        this.day = day;
    }
}
