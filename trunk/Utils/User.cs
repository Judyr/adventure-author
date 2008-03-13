/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 13/03/2008
 * Time: 08:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Methods relating to users.
	/// </summary>
	public static class User
	{
		public static string GetCurrentUser()
		{
			return System.Windows.Forms.SystemInformation.UserName;
		}
		
		
		public static string GetOwner(NWN2GameModule module)
		{
			throw new NotImplementedException();
		}
		
		
		public static bool IsTeacher(string user)
		{
			// TODO serialise (and encrypt?) a list of users
			// have a way of nominating someone as a teacher
			return false;
		}
		
		
		public static bool IdentifyTeacherOrDemandPassword()
		{
			return IdentifyTeacherOrDemandPassword(User.GetCurrentUser());
		}
		
		
		public static bool IdentifyTeacherOrDemandPassword(string user)
		{
			if (User.IsTeacher(user)) { // this user has teacher privileges
				return true;
			}
			else { // this user does not have teacher privileges, but allow the 
				   // actual teacher to type in a password for the user
				TeacherPasswordDialog dialog = new TeacherPasswordDialog();
				dialog.ShowDialog();
				return dialog.ReceivedCorrectPassword;
			}			
		}		
	}
}
