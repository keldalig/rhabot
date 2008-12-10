using System;
using ISXBotHelper.Properties;

namespace ISXBotHelper
{
    internal static class clsFace
    {
        #region Variables

        /// <summary>
        /// Last time we did a face
        /// </summary>
        private static DateTime LastFace = DateTime.MinValue;

        /// <summary>
        /// Last time we did a combat face
        /// </summary>
        private static DateTime LastCombatFace = DateTime.MinValue;

        // last point we faced
        private static PathListInfo.PathPoint LastPoint = null;

        private static PathListInfo.PathPoint LastCombatPoint = null;

        // Variables
        #endregion

        #region Functions

        /// <summary>
        /// Faces the target point. Doesn't handle movement stop/start
        /// </summary>
        /// <param name="target"></param>
        public static void FacePointExCombat(PathListInfo.PathPoint target)
        {
            // exit if no target
            if (target == null)
                return;

            using (new clsFrameLock.LockBuffer())
            {
                double headingdiff = Math.Abs(clsSettings.isxwow.Me.Heading - clsSettings.isxwow.Me.HeadingTo(target.X, target.Y));
                // if we faced recently and the point is same point, then don't face again
                if ((headingdiff < 20) && (LastCombatPoint != null) && (LastCombatPoint.SamePoint(target)) && (LastCombatFace > DateTime.Now))
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.FacePointExCombat, Resources.ThisPointWasRecentlyFaced);
                    return;
                }

                // set lastpoint, last face
                LastCombatPoint = target;
                LastCombatFace = DateTime.Now.AddMilliseconds(1000);

                clsSettings.Logging.AddToLog(Resources.FacePointExCombat, target.ToString());

                // face it
                clsSettings.isxwow.Face(target.X, target.Y, 120);
            }
        }

        /// <summary>
        /// Faces the target point (uses code from WoWbot). Returns false if we are not moving
        /// </summary>
        /// <param name="target"></param>
        public static bool FacePointEx(PathListInfo.PathPoint target)
        {
            PathListInfo.PathPoint myPoint;

            // exit if no target
            if (target == null)
                return false;

            using (new clsFrameLock.LockBuffer())
            {
                double headingdiff = Math.Abs(clsSettings.isxwow.Me.Heading - clsSettings.isxwow.Me.HeadingTo(target.X, target.Y));
                // if we faced recently and the point is same point, then don't face again
                if ((headingdiff < 15) && (LastPoint != null) && (LastPoint.SamePoint(target)) && (LastFace > DateTime.Now))
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.FacePointEx, Resources.ThisPointWasRecentlyFaced);
                    return clsPath.Moving;
                }

                clsSettings.Logging.AddToLog(Resources.FacePointEx, target.ToString());

                // set lastpoint, last face
                LastPoint = target;
                LastFace = DateTime.Now.AddMilliseconds(1000);
                
                // set return result
                bool moving = clsPath.Moving;

                // get the distance to the target
                myPoint = clsCharacter.MyLocation;
                double distance = myPoint.Distance(target);

                // WillCollide checks if we will pass into a radius n (n=2) of X,Y
                bool WillCollide = clsSettings.isxwow.Me.WillCollide(myPoint.X, myPoint.Y, 2);

                // don't turn if we are already facing the general direction
                if ((distance < 7) && (WillCollide))
                    return moving;

                // turn to face location
                if ((distance >= 20) || (headingdiff <= 20))
                {
                    if ((distance < 9) || (headingdiff < 10))
                        clsSettings.isxwow.Face(target.X, target.Y, 120);
                    else
                        clsSettings.isxwow.Face(target.X, target.Y, 90);

                    return moving;
                }

                // face if we didn't face already
                if (!((distance < 20) && (headingdiff > 20)))
                {
                    clsSettings.isxwow.Face(target.X, target.Y, 120);
                    return moving;
                }
            }

            // stop moving if heading is too high
            clsSettings.Logging.AddToLog(Resources.FacePointEx, Resources.StopMoving);

            // this needs to be handled in a new framelock, so the stop will be called
            clsPath.StopMoving();

            // face
            using (new clsFrameLock.LockBuffer())
                clsSettings.isxwow.Face(target.X, target.Y, 120);

            return false;
        }

        // Functions
        #endregion
    }
}
