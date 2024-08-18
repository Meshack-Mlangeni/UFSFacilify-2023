package mncedisi.mlangeni.myrecapp.data;

import java.io.Serializable;
import java.sql.ResultSet;
import java.util.ArrayList;

public class Category implements Serializable
{
   private int CategoryId;
   private String CategoryName;
   private int FacilityId;

   public Category(int categoryId, String categoryName, int facilityId) {
      CategoryId = categoryId;
      CategoryName = categoryName;
      FacilityId = facilityId;
   }

   public int getCategoryId() {
      return CategoryId;
   }

   public void setCategoryId(int categoryId) {
      CategoryId = categoryId;
   }

   public String getCategoryName() {
      return CategoryName;
   }

   public void setCategoryName(String categoryName) {
      CategoryName = categoryName;
   }

   public int getFacilityId() {
      return FacilityId;
   }

   public void setFacilityId(int facilityId) {
      FacilityId = facilityId;
   }

   public static ArrayList<Category> createCategoriesFromResultSet(ResultSet set){
      ArrayList categories = new ArrayList<Facility>();
      try {
         while (set.next()) {
               Category category = new Category(0,"", 0);
               category.setCategoryId(set.getInt(1));
               category.setCategoryName(set.getString(2));
               category.setFacilityId(set.getInt(3));
               categories.add(category);
         }
      } catch (Exception e) {
         throw new RuntimeException(e);
      }
      return categories;
   }
}
