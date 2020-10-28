using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Unit_And_Integration_Tests
    {

        static GameObject gameObject = new GameObject();
        DatabaseMgr db = gameObject.AddComponent<DatabaseMgr>();
        ProfileMgr pm = gameObject.AddComponent<ProfileMgr>();
        SessionMgr sm = gameObject.AddComponent<SessionMgr>();


        /// <summary>
        /// Unittest for Loading profile after email login.
        /// </summary>
        [UnityTest]
        public IEnumerator LoadProfileTest()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            db.EmailLogin(
                "arumugam004@e.ntu.edu.sg",
                "12345678",
                delegate ()
                {
                    Assert.AreEqual(true, db.IsLoggedIn);
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
        /// Unittest for Email Login.
        /// </summary>
        [UnityTest]
        public IEnumerator LoginTest()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            db.EmailLogin(
                "arumugam004@e.ntu.edu.sg",
                "12345678",
                delegate ()
                {
                    Assert.AreEqual(true, db.IsLoggedIn);
                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
            yield return new WaitForSeconds(2);

        }


        /// <summary>
        /// Unittest for Email Logout.
        /// </summary>
        [UnityTest]
        public IEnumerator LogoutTest()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

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

        /// <summary>
        /// Unittest for Question Creation.
        /// </summary>
        [UnityTest]
        public IEnumerator CreateQuestionTest()
        {

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
            Question q = new Question();
            q.id_owner = "aru6";
            q.question = "answer to life, the universe and everything";
            q.answer1 = "40";
            q.answer2 = "41";
            q.answer3 = "42";
            q.answer4 = "43";
            q.correctanswer = 3;
            pm.CreateNewQuestion(
                q,
                delegate (string msg)
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
        /// Unittest for Question List Creation.
        /// </summary>
        [UnityTest]
        public IEnumerator CreateQuestionListTest()
        {

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
            QuestionList ql = new QuestionList();

            ql.size = 0;
            ql.id_owner = "aru6";
            ql.name = "test list";
            pm.CreateNewQuestionList(

                ql,
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
        /// Unittest for Updating a question.
        /// </summary>
        [UnityTest]
        public IEnumerator UpdateQuestionTest()
        {

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

            Question q = new Question();
            q.id_owner = "aru6";
            q.question = "What is answer to life, the universe and everything";
            q.answer1 = "40";
            q.answer2 = "41";
            q.answer3 = "42";
            q.answer4 = "43";
            q.correctanswer = 3;
            pm.UpdateMyQuestion(
                "-MKcQv8-oer2QTKsgn0c", 
                q, 
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
        /// Unittest for updating a question list.
        /// </summary>
        [UnityTest]
        public IEnumerator UpdateQuestionListTest()
        {

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
            QuestionList ql = new QuestionList();

            ql.size = 1;
            ql.id_owner = "aru6";
            ql.list = new List<string>();
            ql.list.Add("-MKcQv8-oer2QTKsgn0c");
            ql.name = "test list";
            pm.UpdateMyQuestionList(
                "-MKcr4ANmRFwa5_ZTbE5",
                ql,
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
        /// Unittest for session creation.
        /// </summary>
        [UnityTest]
        public IEnumerator CreateSessionTest()
        {

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
            Session s = new Session();
            s.id_owner = "aru6";
            s.session_name = "test session unit tests";
            s.id_l1queslist = "-MKcr4ANmRFwa5_ZTbE5";
            s.id_l2queslist = "-MJVoMZ26JNmFYukTBjK";
            s.id_l3queslist = "-MJXPvyGhzXnEO_2SY3Y";

            sm.CreateSession(
                s,
                delegate ()
                {

                }, 
                delegate (string msg)
                {
                    Assert.Fail();
                }
            );
        }

        /// <summary>
        /// Unittest for Fetching a session.
        /// </summary>
        [UnityTest]
        public IEnumerator FetchSessionTest()
        {

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

            sm.FetchSingleSession("-MKcuUvNbaoW6kDWJtQM",
                delegate (string result)
                {
                    Debug.Log(result);
                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
                );

            yield return new WaitForSeconds(3);

        }


        /// <summary>
        /// Unittest for Fetching all sessions.
        /// </summary>
        [UnityTest]
        public IEnumerator FetchAllSessionsTest()
        {

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

            sm.FetchSessions(
                delegate (string result)
                {
                    Debug.Log(result);
                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
                );

            yield return new WaitForSeconds(3);

        }

        /// <summary>
        /// Unittest for Fetching questions for a game.
        /// </summary>
        [UnityTest]
        public IEnumerator FetchQuestionsForGameTest()
        {

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

            sm.FetchQuestionsForGame("-MKcuUvNbaoW6kDWJtQM", 
                1,
                delegate ()
                {
                    Assert.AreEqual("aru6", sm.passedInQuestionList[0].id_owner);
                },
                delegate (string msg)
                {
                    Assert.Fail();
                }
                );

            yield return new WaitForSeconds(3);

        }

    }
}
