// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace ShootingRangeGame.Tags
// {
//     [System.Serializable]
//     public class TagMask
//     {
//         public List<Tag> tags;
//         public Mode mode;
//         public bool invert;
//         public bool fallbackResult;
//
//         public bool Compare(Object obj)
//         {
//             bool getResult()
//             {
//                 var taggable = obj as Taggable;
//                 switch (obj)
//                 {
//                     case Taggable:
//                         break;
//                     case GameObject gameObject:
//                         taggable = gameObject.GetComponent<Taggable>();
//                         break;
//                     case Component component:
//                         taggable = component.GetComponent<Taggable>();
//                         break;
//                 }
//
//                 if (!taggable) return fallbackResult;
//
//                 return mode switch
//                 {
//                     Mode.Superset => Subset(taggable.Tags, tags),
//                     Mode.Subset => Subset(tags, taggable.Tags),
//                     Mode.MatchOne => MatchOne(taggable.Tags),
//                     Mode.Exact => Exact(taggable.Tags),
//                     _ => fallbackResult
//                 };
//             }
//
//             return getResult() != invert;
//         }
//
//         public bool SetOperation(List<Tag> a, List<Tag> b, Func<HashSet<Tag>, HashSet<Tag>, bool> predicate)
//         {
//             var setA = new HashSet<Tag>();
//             var setB = new HashSet<Tag>();
//
//             foreach (var e in a)
//             {
//                 if (setA.Contains(e)) continue;
//                 setA.Add(e);
//             }
//             
//             foreach (var e in b)
//             {
//                 if (setB.Contains(e)) continue;
//                 setB.Add(e);
//             }
//
//             return predicate(setA, setB);
//         }
//         
//         private bool Subset(List<Tag> subset, List<Tag> superset)
//         {
//             foreach (var sub in subset)
//             {
//                 var hasTag = false;
//                 foreach (var super in superset)
//                 {
//                     if (super != sub) continue;
//                     hasTag = true;
//                     break;
//                 }
//
//                 if (!hasTag) return false;
//             }
//
//             return true;
//         }
//
//         private bool MatchOne(List<Tag> otherTags)
//         {
//             foreach (var tag in tags)
//             {
//                 foreach (var other in otherTags)
//                 {
//                     if (tag == other) return true;
//                 }
//             }
//
//             return true;
//         }
//
//         public enum Mode
//         {
//             Superset,
//             Subset,
//             MatchOne,
//             Exact,
//         }
//     }
// }