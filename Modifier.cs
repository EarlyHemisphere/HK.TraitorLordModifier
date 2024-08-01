using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using UnityEngine;

namespace TraitorLordModifier {
    public class Modifier: MonoBehaviour {
        private int attackCounter = 0;
        private PlayMakerFSM mantisFSM = null;

        public void Awake() {
            On.PlayMakerFSM.OnEnable += OnFsmEnable;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneChanged;
        }

        private void SceneChanged(UnityEngine.SceneManagement.Scene _, UnityEngine.SceneManagement.Scene to) {
            if (to.name != "GG_Traitor_Lord") {
                mantisFSM = null;
            }
        }

        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            
            if (self.gameObject.name.Contains("Traitor Lord") && self.FsmName == "Mantis") {
                mantisFSM = self;
                ApplySettings();
            }
        }

        public void ApplySettings() {
            ApplyMaxAttacksBetweenSlams();
            ApplyDoubleSlamPrevention();
        }

        private void ApplyDoubleSlamPrevention() {
            if (mantisFSM == null) return;

            if (TraitorLordModifier.localSettings.preventDoubleSlam) {
                mantisFSM.InsertAction("Repeat?", new CallMethod {
                    behaviour = this,
                    methodName = "SkipSlamAttack",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);
            } else {
                if (mantisFSM.GetState("Repeat?").Actions.Length > 2) {
                    mantisFSM.RemoveAction("Repeat?", 0);
                }
            }
        }

        private void ApplyMaxAttacksBetweenSlams() {
            if (mantisFSM == null) return;

            if (TraitorLordModifier.localSettings.maxAttacksBetweenSlams) {
                mantisFSM.InsertAction("Idle", new CallMethod {
                    behaviour = this,
                    methodName = "IncrementAttackCounter",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);
                mantisFSM.InsertAction("Slam?", new CallMethod {
                    behaviour = this,
                    methodName = "CheckAttackCounter",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 3);
            } else {
                if (mantisFSM.GetState("Idle").Actions.Length > 8) {
                    mantisFSM.RemoveAction("Idle", 0);
                }
                if (mantisFSM.GetState("Slam?").Actions.Length > 4) {
                    mantisFSM.RemoveAction("Slam?", 3);
                }
            }
        }

        public void IncrementAttackCounter() {
            attackCounter++;
        }

        public void CheckAttackCounter() {
            if (attackCounter >= 5) {
                mantisFSM.SendEvent("SLAM");
                attackCounter = 0;
            } else {
                mantisFSM.SendEvent("FINISHED");
            }
        }

        public void SkipSlamAttack() {
            mantisFSM.SendEvent("FINISHED");
        }
    }
}