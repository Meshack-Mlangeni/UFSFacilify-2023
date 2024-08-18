package mncedisi.mlangeni.myrecapp.data;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;

public class Booking implements Serializable {
    private int BookingId;
    private Date DateStart;
    private Date DateEnd;
    private String BookingPass;
    private Boolean isValid;
    private Boolean Approved;
    private String userEmail;
    private int FacilityId;
    private int CategoryId;
    private String FacilityName;
    private String CategoryName;

    public String getFacilityName() {
        return FacilityName;
    }

    public void setFacilityName(String facilityName) {
        FacilityName = facilityName;
    }

    public String getCategoryName() {
        return CategoryName;
    }

    public void setCategoryName(String categoryName) {
        CategoryName = categoryName;
    }

    public Booking(int bookingId, Date dateStart, Date dateEnd, String bookingPass, Boolean isValid, Boolean approved, String userEmail, int facilityId, int categoryId) {
        BookingId = bookingId;
        DateStart = dateStart;
        DateEnd = dateEnd;
        BookingPass = bookingPass;
        this.isValid = isValid;
        Approved = approved;
        this.userEmail = userEmail;
        FacilityId = facilityId;
        CategoryId = categoryId;
    }

    // TODO: 2023/10/21 fix this error 
    public static ArrayList<Booking> createUserBookingsFromSet(ResultSet set, Connection connection) {
        ArrayList<Booking> bookings = new ArrayList<Booking>();
        try {
            while (set.next()) {
                Calendar c = Calendar.getInstance();

                Booking booking = new Booking(0, null, null, "", false, false, "", 0, 0);
                booking.setBookingId(set.getInt(1));

                booking.setDateStart(new SimpleDateFormat("yyyy-MM-dd HH:mm").parse(set.getString(2).substring(0,16)));
                booking.setDateEnd(new SimpleDateFormat("yyyy-MM-dd HH:mm").parse(set.getString(3).substring(0,16)));
                booking.setBookingPass(set.getString(4));
                booking.setValid(set.getBoolean(5));
                booking.setApproved(set.getBoolean(6));
                booking.setUserEmail(set.getString(7));
                booking.setFacilityId(set.getInt(8));
                booking.setCategoryId(set.getInt(9));

                if (connection != null) {
                    Statement statement = connection.createStatement();
                    ResultSet facCatSet = statement.executeQuery(" select CategoryName, Name from Category " +
                            "join Facility on Category.FacilityId = Facility.FacilityId " +
                            "where CategoryId = " + set.getInt(9) + "");
                    try {
                        while (facCatSet.next()) {
                            booking.setCategoryName(facCatSet.getString(1));
                            booking.setFacilityName(facCatSet.getString(2));
                        }
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }

                bookings.add(booking);
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        return bookings;
    }


    public int getBookingId() {
        return BookingId;
    }

    public void setBookingId(int bookingId) {
        BookingId = bookingId;
    }

    public Date getDateStart() {
        return DateStart;
    }

    public void setDateStart(Date dateStart) {
        DateStart = dateStart;
    }

    public Date getDateEnd() {
        return DateEnd;
    }

    public void setDateEnd(Date dateEnd) {
        DateEnd = dateEnd;
    }

    public String getBookingPass() {
        return BookingPass;
    }

    public void setBookingPass(String bookingPass) {
        BookingPass = bookingPass;
    }

    public Boolean getValid() {
        return isValid;
    }

    public void setValid(Boolean valid) {
        isValid = valid;
    }

    public Boolean getApproved() {
        return Approved;
    }

    public void setApproved(Boolean approved) {
        Approved = approved;
    }

    public String getUserEmail() {
        return userEmail;
    }

    public void setUserEmail(String userEmail) {
        this.userEmail = userEmail;
    }

    public int getFacilityId() {
        return FacilityId;
    }

    public void setFacilityId(int facilityId) {
        FacilityId = facilityId;
    }

    public int getCategoryId() {
        return CategoryId;
    }

    public void setCategoryId(int categoryId) {
        CategoryId = categoryId;
    }
}
