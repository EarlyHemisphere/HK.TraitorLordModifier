using Modding;
using Satchel.BetterMenus;
using UnityEngine;

namespace TraitorLordModifier {
    public class TraitorLordModifier : Mod, ICustomMenuMod {
        private Menu menuRef = null;
        public static TraitorLordModifier instance;
        private Modifier modifierComponent = null;

        public TraitorLordModifier() : base("Traitor Lord Modifier") => instance = this;
        public static LocalSettings localSettings { get; private set; } = new();
        public void OnLoadLocal(LocalSettings s) => localSettings = s;
        public LocalSettings OnSaveLocal() => localSettings;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public bool ToggleButtonInsideMenu => false;

        public override void Initialize() {
            Log("Initializing");

            ModHooks.AfterSavegameLoadHook += AfterSaveGameLoad;
            ModHooks.NewGameHook += AddModifierComponent;

            Log("Initialized");
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) {    
            menuRef ??= new Menu(
                name: "Prevent Double Slam Attack",
                elements: new Element[] {
                    new HorizontalOption(
                        name: "Prevent Double Slam Attack",
                        description: "Prevents slam attack from being used twice in a row.",
                        values: new [] {"true", "false"},
                        applySetting: val => {
                            localSettings.preventDoubleSlam = val == 0;
                            if (modifierComponent != null) {
                                modifierComponent.ApplySettings();
                            }
                        },
                        loadSetting: () => localSettings.preventDoubleSlam ? 0 : 1,
                        Id: "preventDoubleSlam"
                    ),
                    new HorizontalOption(
                        name: "Max Attacks Between Slams",
                        description: "Forces five attacks to be used in between slam attacks.",
                        values: new [] {"true", "false"},
                        applySetting: val => {
                            localSettings.maxAttacksBetweenSlams = val == 0;
                            if (modifierComponent != null) {
                                modifierComponent.ApplySettings();
                            }
                        },
                        loadSetting: () => localSettings.maxAttacksBetweenSlams ? 0 : 1,
                        Id: "maxAttacksBetweenSlams"
                    )
                }
            );
            
            return menuRef.GetMenuScreen(modListMenu);
        }

        private void AfterSaveGameLoad(SaveGameData _) {
            AddModifierComponent();
        }

        private void AddModifierComponent() {
            modifierComponent = GameManager.instance.gameObject.AddComponent<Modifier>();
        }

        public class LocalSettings {
            public bool preventDoubleSlam = false;
            public bool maxAttacksBetweenSlams = false;
        }
    }
}