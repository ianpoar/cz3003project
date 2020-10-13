using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {

        /// <summary>
        /// Unittest for Email Login.
        /// </summary>
        [UnityTest]
        public IEnumerator LoginTest()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            var gameObject = new GameObject();
            var db = gameObject.AddComponent<DatabaseMgr>();
            var pm = gameObject.AddComponent<ProfileMgr>();
            yield return new WaitForSeconds(1);
            db.EmailLogin(
                "arumugam004@e.ntu.edu.sg",
                "12345678",
                delegate ()
                {
                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(2);
            pm.LoadPlayerProfile(
                delegate ()
                {
                    Assert.AreEqual("aru6", pm.localProfile.name);
                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );

            yield return new WaitForSeconds(3);

        }

        /// <summary>
        /// Unittest for Email Logout.
        /// </summary>
        [UnityTest]
        public IEnumerator LogoutTest()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            var gameObject = new GameObject();
            var db = gameObject.AddComponent<DatabaseMgr>();
            var pm = gameObject.AddComponent<ProfileMgr>();

            yield return new WaitForSeconds(1);
            db.EmailLogin(
                "arumugam004@e.ntu.edu.sg",
                "12345678",
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(3);
            db.Logout();
            yield return new WaitForSeconds(3);
            Assert.AreEqual(false, db.IsLoggedIn);
        }

        /// <summary>
        /// Unittest for Joining Session.
        /// </summary>
        [UnityTest]
        public IEnumerator JoinSessionTest()
        {
            var gameObject = new GameObject();
            var sm = gameObject.AddComponent<SessionMgr>();
            var db = gameObject.AddComponent<DatabaseMgr>();
            yield return new WaitForSeconds(1);
            db.EmailLogin(
                "arumugam004@e.ntu.edu.sg",
                "12345678",
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(3);
            Connection con = new Connection();
            con.id_owner = "iangoogle";
            con.id_player = "aru6";
            con.id_session = "-MJLSg5sk--UX7gXN6Ce";
            con.level_cleared = 0;
            con.session_name = "Test Session";
            sm.JoinSession(
                con,
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );

            yield return new WaitForSeconds(3);
        }

        /// <summary>
        /// Unittest for Leaving Session.
        /// </summary>
        [UnityTest]
        public IEnumerator LeaveSessionTest()
        {
            var gameObject = new GameObject();
            var sm = gameObject.AddComponent<SessionMgr>();
            var db = gameObject.AddComponent<DatabaseMgr>();
            yield return new WaitForSeconds(1);
            db.EmailLogin(
                "arumugam004@e.ntu.edu.sg",
                "12345678",
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(3);
            Connection con = new Connection();
            con.id_owner = "iangoogle";
            con.id_player = "aru6";
            con.id_session = "-MJLSg5sk--UX7gXN6Ce";
            con.level_cleared = 0;
            con.session_name = "Test Session";
            sm.JoinSession(
                con,
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(3);

            sm.LeaveSession(
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
                );
            yield return new WaitForSeconds(3);

        }

        /// <summary>
        /// Unittest for Report Generation.
        /// </summary>
        [UnityTest]
        public IEnumerator ReportGenerateTest()
        {
            var gameObject = new GameObject();
            var sm = gameObject.AddComponent<SessionMgr>();
            var db = gameObject.AddComponent<DatabaseMgr>();
            yield return new WaitForSeconds(1);
            db.EmailLogin(
                "arumugam004@e.ntu.edu.sg",
                "12345678",
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(3);
            Connection con = new Connection();
            con.id_owner = "iangoogle";
            con.id_player = "aru6";
            con.id_session = "-MJLSg5sk--UX7gXN6Ce";
            con.level_cleared = 0;
            con.session_name = "Test Session";
            sm.JoinSession(
                con,
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(3);

            //sm.LoadAllSessionReports(con.id_session,)
            sm.LeaveSession(
                delegate ()
                {

                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
                );
            yield return new WaitForSeconds(3);

        }

        /*
        [UnityTest]
        public IEnumerator ScreenTransitTest()
        {
            yield return new WaitForSeconds(1);
            UnityEngine.SceneManagement.SceneManager.
        }
        */
    }
}
