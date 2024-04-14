using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using SFCore.Utils;
using UnityEngine;

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
            fsm.GetAction<CheckTargetDirection>("Repeat Pos Check", 0).rightEvent = new FsmEvent("");
            fsm.GetAction<CheckTargetDirection>("Repeat Pos Check", 0).leftEvent = new FsmEvent("");
        }

        public void Unload() {
            GameObject mantis = GameObject.Find("Mantis Traitor Lord");
            if (mantis != null) {
                mantis.LocateMyFSM("Mantis").GetAction<CheckTargetDirection>("Repeat Pos Check", 0).rightEvent = new FsmEvent("R");
                mantis.LocateMyFSM("Mantis").GetAction<CheckTargetDirection>("Repeat Pos Check", 0).leftEvent = new FsmEvent("L");
            }
        }
    }
}