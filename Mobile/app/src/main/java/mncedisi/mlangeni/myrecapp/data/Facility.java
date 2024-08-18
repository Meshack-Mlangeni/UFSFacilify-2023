package mncedisi.mlangeni.myrecapp.data;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

import java.io.Serializable;
import java.math.BigInteger;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;

public class Facility implements Serializable
{
    private int FacilityId;
    private String Name;
    private String description;
    private int Price;
    private String SecurityEmail;
    private boolean Available;
    private String Space;
    private String imgHex;

    public Facility(int facilityId, String name, String description, int price, String securityEmail, boolean available, String space, String _imgHex) {
        FacilityId = facilityId;
        Name = name;
        this.description = description;
        Price = price;
        SecurityEmail = securityEmail;
        Available = available;
        Space = space;
        imgHex = _imgHex;
    }

    public String getImgHex() {
        return imgHex;
    }

    public void setImgHex(String imgHex) {
        this.imgHex = imgHex;
    }

    public int getFacilityId() {
        return FacilityId;
    }

    public void setFacilityId(int facilityId) {
        FacilityId = facilityId;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public int getPrice() {
        return Price;
    }

    public void setPrice(int price) {
        Price = price;
    }

    public String getSecurityEmail() {
        return SecurityEmail;
    }

    public void setSecurityEmail(String securityEmail) {
        SecurityEmail = securityEmail;
    }

    public boolean isAvailable() {
        return Available;
    }

    public void setAvailable(boolean available) {
        Available = available;
    }

    public String getSpace() {
        return Space;
    }

    public void setSpace(String space) {
        Space = space;
    }

    public static ArrayList<Facility> createFacilitiesFromResultSet(ResultSet set, Connection connection){
        ArrayList facilities = new ArrayList<Facility>();
        try {
            while (set.next()) {
                Facility facility = new Facility(0,"", "" , 0,"",false,"", "");
                facility.setFacilityId(set.getInt(1));
                facility.setName(set.getString(2));
                facility.setDescription(set.getString(3));
                facility.setPrice(set.getInt(4));
                facility.setSpace(set.getString(5));
                facility.setSecurityEmail(set.getString(6));
                facility.setAvailable(set.getBoolean(7));

                if(connection != null){
                    Statement statement = connection.createStatement();
                    ResultSet imgResultSet = statement.
                            executeQuery("select top(1) image from image where FacilityId = '" + set.getInt(1) + "'");
                    try
                    {
                        while (imgResultSet.next()){
                            facility.setImgHex(imgResultSet.getString(1));
                        }
                    }catch (SQLException e){ e.printStackTrace();}
                }
                facilities.add(facility);
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        return facilities;
    }

    public Bitmap getImage(){

        if(getImgHex() != null) {
            byte[] bytes = new byte[getImgHex().length() / 2];
            for (int i = 0; i < bytes.length; i++) {
                bytes[i] = (byte) Integer.parseInt(getImgHex().substring(i * 2, i * 2 + 2), 16);
            }
            return BitmapFactory.decodeByteArray(bytes, 0, bytes.length);
        } return null;
    }
}
