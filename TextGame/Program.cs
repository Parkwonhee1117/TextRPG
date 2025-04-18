using System.Reflection.Emit;
using System.Threading;
using System.Xml.Linq;

namespace Sparta
{
    internal class TextGame
    {
        interface IExit
        {
            void Exit();
        }
        // 로그인하여 게임 플레이
        // 만약 처음이라면 회원가입
        // 로그인이 처음이라면 캐릭터 생성 화면 띄우기
        // 로그인에 성공하면 마을,활동창 띄우기
        // 
        public abstract class Scene
        {
            public abstract void EnterMessage();
        }
        public class LogIn : IExit
        {
            public void Security()
            {
                Console.WriteLine("아이디를 입력해주세요.");
            }

            public void MakeId()
            {
                Console.WriteLine("새롭게 만드실 아이디를 적어주세요.");
            }

            public void Greeting(string characterName)
            {
                name = characterName;
                Console.WriteLine("{0}님 환영합니다.", name);
            }

            public void Exit()
            {
                Console.WriteLine("게임을 종료합니다.");
            }

            private string id;
            private string name;
        }

        public class MakeName
        {
            public string Name { get; set; }
        }

        public class Village : Scene
        {
            public override void EnterMessage()
            {
                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.\n" +
                                  "이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n" +
                                  "\r\n1. 상태 보기\r\n2. 인벤토리\r\n3. 상점\r\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">> ");
            }

            public void VillageAction(Status playerStatus, Inventory inventory, Store store)
            {
                Console.Clear();
                EnterMessage();
                string villageChoice = Console.ReadLine();

                switch (villageChoice)
                {
                    case "1":
                        playerStatus.EnterMessage();
                        break;

                    case "2":
                        inventory.EnterMessage();
                        break;

                    case "3":
                        store.EnterMessage();
                        store.OwnGold();
                        store.ShowItems();
                        store.ShowItemsWithNumber();
                        store.BuyItem();
                        break;

                    default:
                        Console.Write("해당하는 숫자를 입력해주세요.");
                        Thread.Sleep(500);
                        VillageAction(playerStatus, inventory, store);
                        break;

                }
            }
        }

        public class Status : Scene, IExit
        {
            MakeName name = new MakeName();

            public Status()
            {
                Level = 1;
                StatusName = name.Name;
                Job = "모험가";
                Power = 10;
                Defence = 5;
                Hp = 100;
                Gold = 5000;
            }
            public override void EnterMessage()
            {
                Console.Clear();
                Console.WriteLine("상태 보기\r\n캐릭터의 정보가 표시됩니다.\n");

                Console.WriteLine($"Lv. {Level}");
                Console.WriteLine($"{StatusName} ({Job})");
                Console.WriteLine($"공격력 : {Power}");
                Console.WriteLine($"방어력 : {Defence}");
                Console.WriteLine($"체 력 : {Hp}");
                Console.WriteLine($"Gold : {Gold}\n");

                Console.WriteLine("0. 나가기\n");

                Console.Write("원하시는 행동을 입력해주세요.\r\n>>");

                Exit();
            }

            public int Level { get; set; }
            public string StatusName { get; set; }
            public string Job { get; set; }
            public int Power { get; set; }
            public int Defence { get; set; }
            public int Hp { get; set; }
            public int Gold { get; set; }


            public void Exit()
            {
                string userInput = Console.ReadLine();

                Village village = new Village();

                if (userInput == "0")
                {
                    Console.Clear();
                }
                else
                {
                    Console.Write("해당하는 숫자를 입력하세요");
                    Thread.Sleep(500);
                    EnterMessage();
                }

            }
        }

        public class Inventory : Scene, IExit
        {
            public List<StoreItem> StoreItems = new List<StoreItem>();

            private Store store;
            private Village village;
            private Status playerStatus;

            public Inventory(Store outsideStore, Village outsideVillage, Status Status)
            {
                store = outsideStore;
                village = outsideVillage;
                playerStatus = Status;
            }


            public override void EnterMessage()
            {
                Console.Clear();
                Console.WriteLine("인벤토리\r\n보유 중인 아이템을 관리할 수 있습니다.\n");
                Console.WriteLine("[아이템 목록]");
                ItemList();
                Console.WriteLine();
                Console.WriteLine("1. 장착 관리");
                Console.WriteLine("0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">> ");

                Exit();
            }

            public void ItemList()
            {
                foreach (StoreItem item in store.StoreItems)
                {
                    if (item.isSold)
                    {
                        Console.WriteLine($"- {item.Name}    | {(item.isWeapons ? "공격력" + item.Effect : "방어력" + item.Effect)}  | {item.Description}");
                    }
                }
            }

            public void EquipManagement()
            {
                int i = 0;

                Dictionary<int, StoreItem> itemDictionaty = new Dictionary<int, StoreItem>();

                Console.Clear();
                Console.WriteLine("인벤토리\r\n보유 중인 아이템을 관리할 수 있습니다.\n");
                Console.WriteLine("[아이템 목록]");

                foreach (StoreItem item in store.StoreItems)
                {
                    if (item.isSold)
                    {
                        i++;
                        item.number = i;
                        itemDictionaty[i] = item;

                        string equippedMark = item.isUse ? "[E]" : "";

                        Console.WriteLine($"{item.number} {equippedMark} {item.Name}    | {(item.isWeapons ? "공격력" + item.Effect : "방어력" + item.Effect)}  | {item.Description}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("0. 나가기\n");
                Console.WriteLine("장착/헤체 하고자 하는 장비를 선택해주세요.");
                Console.Write(">> ");

                string input = Console.ReadLine();

                if (input == "0")
                {
                    Console.Clear();
                    EnterMessage();
                }

                else if (int.TryParse(input, out int selectedNumber) && itemDictionaty.ContainsKey(selectedNumber))
                {
                    StoreItem selectedItem = itemDictionaty[selectedNumber];
                    selectedItem.isUse = !selectedItem.isUse;

                    if (selectedItem.isUse)
                    {
                        Console.WriteLine($"\n{selectedItem.Name} 장착 완료");

                        if (selectedItem.isWeapons)
                        {
                            playerStatus.Power += selectedItem.Effect;
                        }
                        else
                        {
                            playerStatus.Defence += selectedItem.Effect;
                        }
                    }

                    else
                    {
                        Console.WriteLine($"\n{selectedItem.Name} 장착 해제");

                        if (selectedItem.isWeapons)
                        {
                            playerStatus.Power -= selectedItem.Effect;
                        }
                        else
                        {
                            playerStatus.Defence -= selectedItem.Effect;
                        }
                    }

                    Console.Clear();
                    EquipManagement();
                }
                else
                {
                    Console.WriteLine("해당하는 숫자를 입력하세요");
                    Thread.Sleep(500);

                    Console.Clear();
                    EquipManagement();
                }


            }

            public void Exit()
            {
                string userInput = Console.ReadLine();

                if (userInput == "0")
                {
                    Console.Clear();
                }
                else if (userInput == "1")
                {
                    EquipManagement();
                }
                else
                {
                    Console.Write("해당하는 숫자를 입력하세요.");
                    Thread.Sleep(500);
                    EnterMessage();
                }


            }
        }


        public class Store : Scene, IExit
        {
            StoreItem item = new StoreItem();
            Status playerStatus;
            public List<StoreItem> StoreItems = new List<StoreItem>();

            public Store(Status status)
            {
                playerStatus = status;
            }

            public override void EnterMessage()
            {
                Console.Clear();
                Console.WriteLine("상점\r\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            }

            public void OwnGold()
            {

                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{playerStatus.Gold} G\n");

            }

            private void AddItem(string itemName, bool itemKind, int itemEffect, string itemDesc, int itemPrice, bool itemIsSold)
            {
                StoreItems.Add(new StoreItem
                {
                    isWeapons = itemKind,
                    Name = itemName,
                    Effect = itemEffect,
                    Description = itemDesc,
                    Price = itemPrice,
                    isSold = itemIsSold
                });
            }

            public void SaveItem()
            {
                AddItem("수련자 갑옷    ", false, 5, " 수련에 도움을 주는 갑옷입니다.                   ", 1000, false);
                AddItem("무쇠갑옷       ", false, 9, " 무쇠로 만들어져 튼튼한 갑옷입니다.               ", 2000, true);
                AddItem("스파르타의 갑옷", false, 15, "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 3500, false);
                AddItem("낡은 검        ", true, 2, " 쉽게 볼 수 있는 낡은 검 입니다.                  ", 600, false);
                AddItem("청동 도끼      ", true, 5, " 어디선가 사용됐던거 같은 도끼입니다.             ", 1500, false);
                AddItem("스파르타의 창  ", true, 7, " 스파르타의 전사들이 사용했다는 전설의 창입니다.  ", 3000, true);
            }

            public void BuyItem()
            {
                bool stopBuyItems = false;


                while (stopBuyItems == false && saveUserInput == "1")
                {
                    string choiceBuyItem = Console.ReadLine();
                    Console.Clear();

                    switch (choiceBuyItem)
                    {
                        case "0":
                            stopBuyItems = true;
                            break;

                        case "1":
                            CompareWithMoney(int.Parse(choiceBuyItem));
                            ShowItemsWithNumber();
                            break;

                        case "2":
                            CompareWithMoney(int.Parse(choiceBuyItem));
                            ShowItemsWithNumber();
                            break;

                        case "3":
                            CompareWithMoney(int.Parse(choiceBuyItem));
                            ShowItemsWithNumber();
                            break;

                        case "4":
                            CompareWithMoney(int.Parse(choiceBuyItem));
                            ShowItemsWithNumber();
                            break;

                        case "5":
                            CompareWithMoney(int.Parse(choiceBuyItem));
                            ShowItemsWithNumber();
                            break;

                        case "6":
                            CompareWithMoney(int.Parse(choiceBuyItem));
                            ShowItemsWithNumber();
                            break;

                        default:
                            ShowItemsWithNumber();
                            break;

                    }

                    choiceBuyItem = null;

                }
            }

            public void ShowItems()
            {
                Console.WriteLine("[아이템 목록]");
                foreach (StoreItem item in StoreItems)
                {

                    Console.WriteLine($"- {item.Name}    | {(item.isWeapons ? "공격력" + item.Effect : "방어력" + item.Effect)}  | {item.Description}       |  {(item.isSold ? "구매완료" : item.Price + " G")}");
                }

                Console.WriteLine();
                Console.WriteLine("1. 아이템 구매");
                Console.Write("0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");

                string buyItem = Console.ReadLine();
                saveUserInput = buyItem;

                if (saveUserInput != "0" && saveUserInput != "1")
                {
                    Console.Write("해당하는 숫자를 입력하세요.");
                    Thread.Sleep(500);

                    EnterMessage();
                    OwnGold();
                    ShowItems();

                }
            }

            public void CompareWithMoney(int itemNumber)
            {
                if (playerStatus.Gold > StoreItems[itemNumber - 1].Price)
                {
                    StoreItems[itemNumber - 1].isSold = true;
                    playerStatus.Gold -= StoreItems[itemNumber - 1].Price;

                }

                else if (StoreItems[itemNumber - 1].isSold == true)
                {
                    ShowItemsWithNumber();
                    Console.WriteLine("이미 구매하셨습니다.");
                    Thread.Sleep(1000);
                }

                else
                {
                    ShowItemsWithNumber();
                    Console.WriteLine("돈이 부족합니다.");
                    Thread.Sleep(1000);
                }
            }

            public void ShowItemsWithNumber()
            {
                Console.Clear();

                if (saveUserInput == "1")
                {
                    EnterMessage();
                    OwnGold();

                    Console.WriteLine("[아이템 목록]");
                    int i = 1;
                    foreach (StoreItem item in StoreItems)
                    {

                        Console.WriteLine($"- {i} {item.Name}    | {(item.isWeapons ? "공격력" + item.Effect : "방어력" + item.Effect)}  | {item.Description}       |  {(item.isSold ? "구매완료" : item.Price + " G")}");
                        i++;
                    }

                    Console.WriteLine();
                    Console.Write("0. 나가기\r\n\r\n구매하고자 하는 물품(번호)을 입력해주세요.\r\n>>");
                }
            }

            public void Exit()
            {
                string userInput = Console.ReadLine();
                if (userInput == "0")
                {
                    Console.Clear();
                }
            }

            string saveUserInput;
        }

        public class StoreItem
        {
            public int number = 0;
            public string Name;
            public int Effect;
            public string Description;
            public int Price;

            public bool isWeapons = false;
            public bool isSold = false;
            public bool isUse = false;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("캐릭터를 생성하세요.");
            MakeName name = new MakeName(); // 이름 만들기
            name.Name = Console.ReadLine();
            Console.Clear();

            LogIn logIn = new LogIn(); // 환영 인사
            logIn.Greeting(name.Name);
            Console.WriteLine();

            Status playerStatus = new Status(); // 초기 스테이터스 설정
            playerStatus.StatusName = name.Name;

            Store store = new Store(playerStatus);
            Village village = new Village();

            Inventory inventory = new Inventory(store, village, playerStatus);
            store.SaveItem();
            

            bool gameStart = false;


            Console.WriteLine("게임을 시작하겠습니까?\n1, 시작하기       2. 그만두기");

            string startChoice = Console.ReadLine();
            Console.Clear();

            do
            {
                gameStart = true;

                if (startChoice == "1")
                {
                    gameStart = true;
                    village.EnterMessage();

                    village.VillageAction(playerStatus, inventory, store);

                }
                else if (startChoice == "2")
                {
                    logIn.Exit();
                    gameStart = false;
                    Console.WriteLine("게임을 종료합니다.");
                }
                else
                {
                    Console.WriteLine("숫자를 입력하세요.");
                    Console.WriteLine("1, 시작하기       2. 그만두기");

                    startChoice = Console.ReadLine();
                    Console.Clear();
                }
            } while (gameStart);

            // 구매, 적용, isSold이용해서 해결하기
            // Exit 깔끔하게 정리하기
        }
    }
}
