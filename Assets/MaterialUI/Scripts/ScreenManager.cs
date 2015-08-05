//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialUI
{
	public class ScreenManager : MonoBehaviour
	{
		public ScreenConfig[] screens;
        public ScreenConfig homeScreen;
		[HideInInspector]
		public ScreenConfig currentScreen;
		[HideInInspector]
		public Stack<ScreenConfig> lastScreens;

        void Start()
        {
            init();
        }

        private void init()
        {
            if (screens == null)
                throw new NullReferenceException("Screen not set");
            if (lastScreens == null)
                lastScreens = new Stack<ScreenConfig>();
            if (homeScreen == null)
                throw new NullReferenceException("Home screen not set");
            if (currentScreen == null)
                SetCurrentScreen(homeScreen);
        }

        private ScreenConfig GetLastScreen()
        {
            if (lastScreens.Count > 0)
            {
                return lastScreens.Pop();
            }
            return homeScreen;
        }

        private void SetLastScreen(ScreenConfig screen)
        {
            lastScreens.Push(screen);
        }

        private void ClearLastScreens()
        {
            lastScreens.Clear();
        }

        /// <summary>
        /// Set active screen
        /// </summary>
        /// <param name="screen"></param>
        private void SetScreen(ScreenConfig screen)
        {
            if (screen == null) return;
            screen.transform.SetAsLastSibling();
            screen.Show(currentScreen);
            SetCurrentScreen(screen);
        }

        /// <summary>
        /// Set current screen variable
        /// </summary>
        /// <param name="screen"></param>
        private void SetCurrentScreen(ScreenConfig screen)
        {
            if (currentScreen != null && currentScreen.associatedMenu != null)
                currentScreen.associatedMenu.UnsetActive();
            currentScreen = screen;
            if (currentScreen == homeScreen) ClearLastScreens();
            if (currentScreen.associatedMenu != null)
                currentScreen.associatedMenu.SetActive();
        }

        /// <summary>
        /// Get screen from menu
        /// </summary>
        /// <param name="ScreenName"></param>
        /// <returns></returns>
        private ScreenConfig GetScreen(NavMenuConfig navMenu)
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].associatedMenu == navMenu)
                {
                    return screens[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Get screen from screen name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ScreenConfig GetScreen(string name)
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].screenName == name)
                {
                    return screens[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Used from navigation bar
        /// History will not be preserved
        /// </summary>
        /// <param name="navMenu"></param>
        public void NavMenuClick(NavMenuConfig navMenu)
        {
            ClearLastScreens();
            SetScreen(GetScreen(navMenu));
        }

        /// <summary>
        /// Used to navigate from within the screen
        /// History of screens will be preserved
        /// </summary>
        /// <param name="name"></param>
        public void Navigate(string name)
        {
            SetLastScreen(currentScreen);
            var screen = GetScreen(name);
            if (screen != null)
                SetScreen(screen);
        }

        /// <summary>
        /// Navigate Back
        /// </summary>
		public void Back()
		{
            var lastScreen = GetLastScreen();
			lastScreen.ShowWithoutTransition();
			currentScreen.Hide();
            SetCurrentScreen(lastScreen);
		}
	}
}