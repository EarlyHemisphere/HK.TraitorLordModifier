using Modding;
using UnityEngine;
using SFCore.Utils;

namespace TraitorLordModifier {
    public class TraitorLordModifier : Mod, ITogglableMod {
        public static TraitorLordModifier instance;

        public TraitorLordModifier() : base("Traitor Lord Modifier") => instance = this;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public bool ToggleButtonInsideMenu => true;

        public override void Initialize() {
            Log("Initializing");

            On.PlayMakerFSM.OnEnable += OnFsmEnable;

            GameObject mantis = GameObject.Find("Mantis Traitor Lord");
            if (mantis != null) {
                ModifyFsm(mantis.LocateMyFSM("Mantis"));
            }

            Log("Initialized");
        }

        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            
            if (self.gameObject.name.Contains("Traitor Lord") && self.FsmName == "Mantis") {
                ModifyFsm(self);
            }
        }

        private void ModifyFsm(PlayMakerFSM fsm) {
            fsm.GetState("Waves").ChangeTransition("FINISHED", "Slam End");
        }

        public void Unload() {
            GameObject mantis = GameObject.Find("Mantis Traitor Lord");
            if (mantis != null) {
                mantis.LocateMyFSM("Mantis").GetState("Waves").ChangeTransition("FINISHED", "Repeat Pos Check");
            }
        }
    }
}