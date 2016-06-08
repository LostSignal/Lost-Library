import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import com.amazonaws.AmazonClientException;
import com.amazonaws.AmazonServiceException;
import com.amazonaws.auth.BasicAWSCredentials;
import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.AmazonS3Client;
import com.amazonaws.services.s3.model.PutObjectRequest;

public class Main 
{
    public static List<File> getAllFiles(File directory)
    {
        List<File> resultList = new ArrayList<File>();
        
        for (File file : directory.listFiles()) 
        {
            if (file.isFile()) 
            {
            	resultList.add(file);
            } 
            else if (file.isDirectory()) 
            {
                resultList.addAll(getAllFiles(file));
            }
        }

        return resultList;
    } 

	public static void main(String[] args) throws IOException
	{
		String accessKeyId = args[0];
		String secreteAccessKey = args[1];
		String bucketName = args[2];
		String assetBundleDirectoryPath = args[3];
		String assetBundleBucketDirectoryPath = args.length > 4 ? args[4] : "";
		
        AmazonS3 s3client = new AmazonS3Client(new BasicAWSCredentials(accessKeyId, secreteAccessKey));
        
        try
        {
        	File assetBundleDirectory = new File(assetBundleDirectoryPath);
        	String fullAssetBundleDirectoryPath = assetBundleDirectory.getAbsolutePath();
        	
        	for (File assetBundleFile : getAllFiles(assetBundleDirectory))
        	{
        		String fullAssetBundlePath = assetBundleFile.getAbsolutePath();
        		String key = assetBundleBucketDirectoryPath + fullAssetBundlePath.substring(fullAssetBundleDirectoryPath.length() + 1);
        		
        		// making sure they're all forward slashes
        		key = key.replace('\\', '/');
        		
        		System.out.println("Uploading AssetBundle: " + key);
        		s3client.putObject(new PutObjectRequest(bucketName, key, assetBundleFile));
        	}
        } 
        catch (AmazonServiceException ase) 
        {
            System.out.println(
	    		"Caught an AmazonServiceException, which " +
	    		"means your request made it " +
	            "to Amazon S3, but was rejected with an error response" +
	            " for some reason.");
	    
            System.out.println("Error Message:    " + ase.getMessage());
            System.out.println("HTTP Status Code: " + ase.getStatusCode());
            System.out.println("AWS Error Code:   " + ase.getErrorCode());
            System.out.println("Error Type:       " + ase.getErrorType());
            System.out.println("Request ID:       " + ase.getRequestId());
        } 
        catch (AmazonClientException ace) 
        {
            System.out.println("Caught an AmazonClientException, which " +
            		"means the client encountered " +
                    "an internal error while trying to " +
                    "communicate with S3, " +
                    "such as not being able to access the network.");
            
            System.out.println("Error Message: " + ace.getMessage());
        }
    }
}
