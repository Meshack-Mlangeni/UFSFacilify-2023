package mncedisi.mlangeni.myrecapp.adapters;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Handler;
import android.os.StrictMode;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.Statement;

import androidx.core.app.ActivityCompat;

public class CreateConnection {
    private Activity context;
    private static final String
            ip_address = "--Enter IP For DB",
            port = "1433", Classes = "net.sourceforge.jtds.jdbc.Driver",
            database = "DATABASE_NAME", username = "DATABASE_USERNAME",
            password = "PASSWORD",
            url = "jdbc:jtds:sqlserver://sql5054.site4now.net/" + database;
    private static Connection connection = null;
    public CreateConnection(Activity _context){
        context = _context;
        ActivityCompat.requestPermissions(context, new String[]{Manifest.permission.INTERNET,
                        Manifest.permission.ACCESS_NETWORK_STATE},
                PackageManager.PERMISSION_GRANTED);

        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);
    }
    public Connection getConnection()
    {
        try {
            Class.forName(Classes);

            connection = DriverManager.getConnection(url, username, password);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return connection;
    }

    public ResultSet getData(String query) {
        ResultSet resultSet = null;
        try {
            Statement statement = connection.createStatement();
            resultSet = statement.executeQuery(query);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return resultSet;
    }
}
