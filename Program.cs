namespace code_1;
class Program
{
    static string renderScreen(string renderedMap, int viewDistance, string[] terrainMap, int[] playerLocation, int health, System.ConsoleKey inputKey, string[,] animationEntitiesMap)
    //render what to display on the screen, and put it in the renderedMap variable
    {
        renderedMap+="┌";
        for(int i = 0; i < viewDistance*2+1; i++)renderedMap+="--";//game area border generation
        renderedMap+="┐\n";
        for (int a = 0; a < viewDistance*2+1; a++)
        {
            renderedMap+="|";
            for (int b = 0; b < viewDistance*2+1; b++)
            {
                try
                {
                    switch(terrainMap[playerLocation[0]-viewDistance+a][playerLocation[1]-viewDistance+b], a==viewDistance && a==b,animationEntitiesMap[playerLocation[0]-viewDistance+a,playerLocation[1]-viewDistance+b], inputKey)
                    //takes data from the terrain map and entity map, and decides what to display based on the player location
                    {
                        case ('0', false, null, not ConsoleKey.None):
                            renderedMap+="  ";
                            break;
                        case ('1', false, not "~", not ConsoleKey.None):
                            renderedMap+="██";
                            break;
                        case ('2', false, null, not ConsoleKey.None):
                            renderedMap+="▓▓";
                            break;
                        case ('3', false, not "~", not ConsoleKey.None):
                            renderedMap+="()";
                            break;
                        case('0', true, null, not ConsoleKey.None):
                            renderedMap+="@ ";
                            break;
                        case('2', true, null, not ConsoleKey.None):
                            renderedMap+="@▓";
                            break;
                        case('0', false, "1", ConsoleKey.UpArrow):
                            renderedMap+="^ ";
                            break;
                        case('2', false, "1", ConsoleKey.UpArrow):
                            renderedMap+="^▓";
                            break;
                        case('0', false, "1", ConsoleKey.RightArrow):
                            renderedMap+="> ";
                            break;
                        case('2', false, "1", ConsoleKey.RightArrow):
                            renderedMap+=">▓";
                            break;
                        case('0', false, "1", ConsoleKey.DownArrow):
                            renderedMap+="v ";
                            break;
                        case('2', false, "1", ConsoleKey.DownArrow):
                            renderedMap+="v▓";
                            break;
                        case('0', false, "1", ConsoleKey.LeftArrow):
                            renderedMap+="< ";
                            break;
                        case('2', false, "1", ConsoleKey.LeftArrow):
                            renderedMap+="<▓";
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    renderedMap+="  ";
                }
            }
            switch(a)
            {
                case 0:
                    renderedMap+="| health: "+health;
                    break;
                default:
                    renderedMap+="|";
                    break;
            }
            renderedMap+="\n";
        }
        renderedMap+="└";
        for(int i = 0; i < viewDistance*2+1; i++)renderedMap+="--";//game area border generation
        renderedMap+="┘\n";
        
        return renderedMap;
    }
    static void Main(string[] args)
    {
        bool end = false; //Manually ended game?
        string[] terrainMap = File.ReadAllLines("map_data.txt"); //Get terrain from file
        string[,] animationEntitiesMap = new string[terrainMap.Length, terrainMap[0].Length]; //animation entity map variable
        string renderedMap = ""; //Rendered map variable
        int[] playerLocation = [19,11]; //Player starting location
        int health = 5; //Player starting health
        int viewDistance = 11; //Player view distance
        System.ConsoleKey inputKey = ConsoleKey.Q; //Input Key
        bool validInput; //Is the input a valid input?
        string playerStatus = "taking a turn"; //Current player status
        Console.CursorVisible = false;
        while (!end) //Game
        {
            switch(playerStatus) //move/attack
            {
                case "taking a turn":
                    switch(inputKey)
                    {
                        case ConsoleKey.UpArrow:
                            animationEntitiesMap[playerLocation[0]-1,playerLocation[1]] = "1";
                            break;
                        case ConsoleKey.RightArrow:
                            animationEntitiesMap[playerLocation[0],playerLocation[1]+1] = "1";
                            break;
                        case ConsoleKey.DownArrow:
                            animationEntitiesMap[playerLocation[0]+1,playerLocation[1]] = "1";
                            break;
                        case ConsoleKey.LeftArrow:
                            animationEntitiesMap[playerLocation[0],playerLocation[1]-1] = "1";//creates a 'sword swing' entity ("1")
                            break;
                        case ConsoleKey.W:
                            if (playerLocation[0] > 0 && (terrainMap[playerLocation[0]-1][playerLocation[1]] is '0' || terrainMap[playerLocation[0]-1][playerLocation[1]] is '2')) playerLocation[0]--;
                            break;
                        case ConsoleKey.D:
                            if (playerLocation[1] < terrainMap[0].Length - 1 && (terrainMap[playerLocation[0]][playerLocation[1]+1] is '0' || terrainMap[playerLocation[0]][playerLocation[1]+1] is '2')) playerLocation[1]++;
                            break;
                        case ConsoleKey.S:
                            if (playerLocation[0] < terrainMap.Length - 1 && (terrainMap[playerLocation[0]+1][playerLocation[1]] is '0' || terrainMap[playerLocation[0]+1][playerLocation[1]] is '2')) playerLocation[0]++;
                            break;
                        case ConsoleKey.A:
                            if (playerLocation[1] > 0 && (terrainMap[playerLocation[0]][playerLocation[1]-1] is '0' || terrainMap[playerLocation[0]][playerLocation[1]-1] is '2')) playerLocation[1]--;//updates the player location, if there is no rock or wall in the way
                            break;
                            
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            if (terrainMap[playerLocation[0]][playerLocation[1]] is '2') health--;//Take damage fom lava
            Console.SetCursorPosition(0,0);
            Console.Write(renderScreen(renderedMap,viewDistance,terrainMap,playerLocation,health,inputKey,animationEntitiesMap));//displays the rendered map in the console
            while(Console.KeyAvailable) Console.ReadKey(true);
            Thread.Sleep(200);
            Console.SetCursorPosition(0,0);
            Array.Clear(animationEntitiesMap);
            Console.Write(renderScreen(renderedMap,viewDistance,terrainMap,playerLocation,health,inputKey,animationEntitiesMap));//displays the rendered map again, once the animation entities have disappeared, and the enemies have taken their turn
            if(health <= 0){Console.Write("       "); break;}//If dead
            do //Accept input
            {   
                inputKey = Console.ReadKey(true).Key;
                switch (inputKey)
                {
                    case ConsoleKey.W or ConsoleKey.D or ConsoleKey.S or ConsoleKey.A or ConsoleKey.Q or ConsoleKey.UpArrow or ConsoleKey.DownArrow or ConsoleKey.RightArrow or ConsoleKey.LeftArrow:
                        playerStatus = "taking a turn";
                        validInput = true;
                        break;
                    case ConsoleKey.E:
                        end = true;
                        validInput = true;
                        break;
                    default:
                        Console.Write("Invald input. Please try again.\n");
                        validInput = false;
                        break;
                }
            }
            while(!validInput);
            Console.WriteLine("                              \n                               \n                               \n");
        }
        Console.Write("Game over. Press any key to exit.");
        Console.ReadKey();
    }
}