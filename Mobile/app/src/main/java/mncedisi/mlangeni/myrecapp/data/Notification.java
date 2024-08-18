package mncedisi.mlangeni.myrecapp.data;

import java.io.Serializable;
import java.sql.ResultSet;
import java.util.ArrayList;
import java.util.Date;

public class Notification implements Serializable
{
    private int Id;
    private String Message;
    private Date Created;
    private boolean isRead;
    private String userEmail;
    public Notification(int id, String message, Date created, boolean isRead, String userEmail) {
        Id = id;
        Message = message;
        Created = created;
        this.isRead = isRead;
        this.userEmail = userEmail;
    }

    public static ArrayList<Notification> createUserNotificationsFromSet(ResultSet set)
    {
        ArrayList<Notification> notifications = new ArrayList<Notification>();
        try {
            while (set.next()) {
                Notification notification = new Notification(0,"", null, false, "");
                notification.setId(set.getInt(1));
                notification.setMessage(set.getString(2));
                notification.setCreated(set.getDate(3));
                notification.setRead(set.getBoolean(4));
                notification.setUserEmail(set.getString(5));
                notifications.add(notification);
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        return notifications;
    }

    public int getId() {
        return Id;
    }

    public void setId(int id) {
        Id = id;
    }

    public String getMessage() {
        return Message;
    }

    public void setMessage(String message) {
        Message = message;
    }

    public Date getCreated() {
        return Created;
    }

    public void setCreated(Date created) {
        Created = created;
    }

    public boolean isRead() {
        return isRead;
    }

    public void setRead(boolean read) {
        isRead = read;
    }

    public String getUserEmail() {
        return userEmail;
    }

    public void setUserEmail(String userEmail) {
        this.userEmail = userEmail;
    }
}
