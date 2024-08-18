package mncedisi.mlangeni.myrecapp.data;

import androidx.annotation.NonNull;

import java.sql.ResultSet;
import java.sql.SQLException;

public class User
{
   private String Id; //1
   private String FirstName; //2
   private String LastName; //3
   private String IdNumber; //4
   private String StudentNumber; //7
   private String Username; //10
   private String NormalizedEmail; //12

   public User(String id, String firstName, String lastName, String idNumber, String studentNumber, String username, String normalizedEmail) {
      Id = id;
      FirstName = firstName;
      LastName = lastName;
      IdNumber = idNumber;
      StudentNumber = studentNumber;
      Username = username;
      NormalizedEmail = normalizedEmail;
   }

   @NonNull
   @Override
   public String toString() {
      return FirstName + " " + LastName;
   }

   public String getId() {
      return Id;
   }

   public void setId(String id) {
      Id = id;
   }

   public String getFirstName() {
      return FirstName;
   }

   public void setFirstName(String firstName) {
      FirstName = firstName;
   }

   public String getLastName() {
      return LastName;
   }

   public void setLastName(String lastName) {
      LastName = lastName;
   }

   public String getIdNumber() {
      return IdNumber;
   }

   public void setIdNumber(String idNumber) {
      IdNumber = idNumber;
   }

   public String getStudentNumber() {
      return StudentNumber;
   }

   public void setStudentNumber(String studentNumber) {
      StudentNumber = studentNumber;
   }

   public String getUsername() {
      return Username;
   }

   public void setUsername(String username) {
      Username = username;
   }

   public String getNormalizedEmail() {
      return NormalizedEmail;
   }

   public void setNormalizedEmail(String normalizedEmail) {
      NormalizedEmail = normalizedEmail;
   }

   public static User createUserFromResultSet(ResultSet set){
      User user = new User(null,null,null,null,null,null,null);
      try {
            while (set.next()) {
               user.setId(set.getString(1));
               user.setFirstName(set.getString(2));
               user.setLastName(set.getString(3));
               user.setNormalizedEmail(set.getString(12));
               user.setIdNumber(set.getString(4));
               user.setStudentNumber(set.getString(7));
               user.setUsername(set.getString(10));
               break;
            }
         } catch (Exception e) {
            throw new RuntimeException(e);
         }
      return user;
   }
}
