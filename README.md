**************** READ ME ****************

I. OVERVIEW:

    My apporach to the problem was the following:
       a. Download the respected zip files.
       b. Extract contents.
       c. Load contents into SQL.
       d. Execute the stored procedure.

   There are things to be aware ofin my solution.

   A. I opted to not use Entity Framework in this situation. It really would not have much a performance factor as far as I am concerned.
   B. The structure of my program and method could be re-written to follow a Production envirnment setting as follows:
         1.   For the download portion of the files, using a HttpClient is completely fine but SFTP is definitely preferred.
      
        URL's were supplied so this approach could not be taken. **
      
         2.  Re-write these methods/functions to be in an API format. As of right now they are not.
      
         3.  Use ILogger functionality; since I am using a Console, I decided to Write to Console for message logs.

         4.  On the data load into SQL table things could have been done to enhance the perforance. All my attempts to try these methods failed.
              *  Spilt .csv's into different sectons. i.e 4 parts. Process each part and have threads do this simulataneously.
              ** Remove all Triggers/Constraints on tables;  opted to not do this since we needed to create a stored procedure as part of problem.
              *** Ensure the clustered index is inserting new records at end of table OR move the clustered index to a column that is frequently used
                        for ordering / joining.
              ****  Change the recovery model of the database to be BULK_LOGGED during the load. Reduce the amount of data being logged in transaction log.
              ***** Use TABLOCK as a query hint or enable the table option table lock on bulk load, to hold a lock for the duration of the bulk-import operation and reduce lock contention.
                  THIS WOULD REQUIRE STORED PROCEDURE 
              ****** Specify ORDER BY clause in the bulk insert operation if the target has a clustered index.
              

II. Other Approaches; not using C#

    A. Use SSIS.
        
    B. Use powershell script with it's 'bcp' command

These are the enhancements that I would take, along with others, if this were to be given a Production environment setting in a future time.
