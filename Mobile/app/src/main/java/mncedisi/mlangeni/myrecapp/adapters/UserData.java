package mncedisi.mlangeni.myrecapp.adapters;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.ResultSet;
import java.util.ArrayList;

import mncedisi.mlangeni.myrecapp.data.Facility;
import mncedisi.mlangeni.myrecapp.data.Notification;

public class UserData implements Serializable {
   private String userEmail;
   private String userId;

   public UserData(String userEmail, String Id) {
      this.userEmail = userEmail; this.userId = Id;
   }

   public String getUserId() {
      return userId;
   }

   public void setUserId(String userId) {
      this.userId = userId;
   }

   public String getUserEmail() {
      return userEmail;
   }

   public void setUserEmail(String userEmail) {
      this.userEmail = userEmail;
   }

}
