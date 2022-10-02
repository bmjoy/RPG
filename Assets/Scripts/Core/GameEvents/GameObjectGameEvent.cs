using System;
using UnityEngine;

namespace RPGEngine.Core
{
    [CreateAssetMenu(fileName = "GameObjectGameEvent", menuName = "RPG Project/Game Events/Game Object Game Event")]
    public class GameObjectGameEvent : GameEvent<GameObject> {}
}