using RimWorld;
using UnityEngine;
using Verse;

namespace OpenWorld
{
    public class MP_MainTabWindow_Online : MainTabWindow
    {
        public override Vector2 RequestedTabSize => new Vector2(586f, 300f);

        private int startAcceptingInputAtFrame;
        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        private Vector2 scrollPosition = Vector2.zero;

        private float sendButtonX = 100f;
        private float sendButtonY = 30f;

        private float textFieldY = 30f;
        private int maxFieldCharacters = 256;

        private string connectionString = "";
        private string userString = "";

        public MP_MainTabWindow_Online()
        {
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            Text.Font = GameFont.Small;

            if (Networking.isConnectedToServer) connectionString = "Status: Connected [" + Main._ParametersCache.playerCount + "]";
            else connectionString = "Status: Disconnected";
            Widgets.Label(new Rect(new Vector2(rect.x, rect.y), new Vector2(Text.CalcSize(connectionString).x, Text.CalcSize(connectionString).y)), connectionString);

            userString = "User: " + Networking.username;
            Widgets.Label(new Rect(new Vector2(rect.xMax - Text.CalcSize(userString).x, rect.y), new Vector2(Text.CalcSize(userString).x, Text.CalcSize(userString).y)), userString);

            try { GenerateList(new Rect(new Vector2(rect.x, rect.y + 25f), new Vector2(rect.width, rect.height - 47f - 25f))); }
            catch { }

            Rect inputAreaRect = new Rect(rect.x, rect.yMax - textFieldY, rect.width - sendButtonX - StandardMargin, textFieldY);
            string inputAreaText = Widgets.TextField(inputAreaRect, Main._MPChat.cacheInputText);
            if (AcceptsInput && inputAreaText.Length <= maxFieldCharacters) Main._MPChat.cacheInputText = inputAreaText;

            Rect sendButtonRect = new Rect(new Vector2(rect.xMax - sendButtonX, rect.yMax - sendButtonY), new Vector2(sendButtonX, sendButtonY));
            bool keyPressed = (inputAreaText.Length > 0) && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
            if (Widgets.ButtonText(sendButtonRect, "Send") || keyPressed)
            {
                if (Main._MPChat.cacheInputText.Contains("│")) Main._MPChat.cacheInputText = Main._MPChat.cacheInputText.Replace("│", "");
                if (Main._MPChat.cacheInputText.Contains("»")) Main._MPChat.cacheInputText = Main._MPChat.cacheInputText.Replace("»", "");
                if (Main._MPChat.cacheInputText.Contains("┼")) Main._MPChat.cacheInputText = Main._MPChat.cacheInputText.Replace("┼", "");
                if (string.IsNullOrWhiteSpace(Main._MPChat.cacheInputText)) return;

                Main._MPChat.SendMessage();
            }
        }

        private void GenerateList(Rect mainRect)
        {
            float height = 6f;

            foreach (string str in Main._MPChat.cacheChatText) height += Text.CalcHeight(str, mainRect.width);

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;
            foreach (string str in Main._MPChat.cacheChatText)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, Text.CalcHeight(str, mainRect.width));
                    DrawCustomRow(rect, str, index);
                }

                num += Text.CalcHeight(str, mainRect.width) + 6f;
                num4++;
                index++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string message, int index)
        {
            Text.Font = GameFont.Small;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));
            if (index % 2 == 0) Widgets.DrawHighlight(fixedRect);
            Widgets.Label(fixedRect, message);
        }
    }
}
