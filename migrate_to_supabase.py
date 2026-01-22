#!/usr/bin/env python3
"""
Migration script from local PostgreSQL to Supabase for EduKin database
"""

import psycopg2
from psycopg2.extras import execute_values
import sys
import logging
from datetime import datetime

# Configuration
LOCAL_POSTGRES_CONFIG = {
    'host': '127.0.0.1',
    'port': 3636,
    'user': 'postgres',
    'password': 'PgLoader2026',
    'database': 'ecole_db'
}

SUPABASE_CONFIG = {
    'host': 'aws-1-eu-central-1.pooler.supabase.com',
    'port': 6543,
    'user': 'postgres.jpprhzgfucwfzmutjrsp',
    'password': 'Polochon1991!!',
    'database': 'postgres'
}

# Logging setup
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler(f'supabase_migration_{datetime.now().strftime("%Y%m%d_%H%M%S")}.log'),
        logging.StreamHandler(sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

def test_supabase_connection():
    """Test connection to Supabase"""
    try:
        conn = psycopg2.connect(**SUPABASE_CONFIG)
        cursor = conn.cursor()
        
        # Test basic query
        cursor.execute("SELECT version()")
        version = cursor.fetchone()[0]
        logger.info(f"Supabase PostgreSQL Version: {version}")
        
        cursor.close()
        conn.close()
        
        logger.info("✅ Supabase connection test successful")
        return True
        
    except Exception as e:
        logger.error(f"❌ Supabase connection test failed: {e}")
        return False

def create_schema_and_tables(supabase_conn):
    """Create ecole_db schema and tables in Supabase"""
    try:
        cursor = supabase_conn.cursor()
        
        # Create schema
        cursor.execute("CREATE SCHEMA IF NOT EXISTS ecole_db")
        logger.info("✅ Created ecole_db schema")
        
        # Get local table structures
        local_conn = psycopg2.connect(**LOCAL_POSTGRES_CONFIG)
        local_cursor = local_conn.cursor()
        
        # Get all tables from local database
        local_cursor.execute("""
            SELECT table_name 
            FROM information_schema.tables 
            WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
            ORDER BY table_name
        """)
        tables = [table[0] for table in local_cursor.fetchall()]
        
        logger.info(f"Found {len(tables)} tables to migrate")
        
        # Create each table in Supabase
        for table_name in tables:
            logger.info(f"Creating table: {table_name}")
            
            # Get table structure
            local_cursor.execute(f"""
                SELECT column_name, data_type, is_nullable, column_default
                FROM information_schema.columns 
                WHERE table_name = '{table_name}' AND table_schema = 'public'
                ORDER BY ordinal_position
            """)
            columns = local_cursor.fetchall()
            
            # Get primary keys
            local_cursor.execute(f"""
                SELECT column_name
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage kcu 
                    ON tc.constraint_name = kcu.constraint_name
                WHERE tc.table_name = '{table_name}' 
                    AND tc.table_schema = 'public'
                    AND tc.constraint_type = 'PRIMARY KEY'
            """)
            primary_keys = [pk[0] for pk in local_cursor.fetchall()]
            
            # Build CREATE TABLE statement
            create_sql = f'CREATE TABLE ecole_db."{table_name}" ('
            column_definitions = []
            
            for col in columns:
                col_name = col[0]
                col_type = col[1]
                col_nullable = col[2] == 'YES'
                col_default = col[3]
                
                # Build column definition
                col_def = f'"{col_name}" {col_type}'
                if not col_nullable:
                    col_def += ' NOT NULL'
                if col_default is not None:
                    col_def += f' DEFAULT {col_default}'
                
                column_definitions.append(col_def)
            
            # Add primary key
            if primary_keys:
                pk_cols = ', '.join([f'"{pk}"' for pk in primary_keys])
                column_definitions.append(f'PRIMARY KEY ({pk_cols})')
            
            create_sql += ', '.join(column_definitions) + ')'
            
            # Drop table if exists and create new one
            cursor.execute(f'DROP TABLE IF EXISTS ecole_db."{table_name}" CASCADE')
            cursor.execute(create_sql)
            
        # Commit schema changes
        supabase_conn.commit()
        
        local_cursor.close()
        local_conn.close()
        cursor.close()
        
        logger.info("✅ Schema and tables created successfully")
        return True
        
    except Exception as e:
        logger.error(f"❌ Failed to create schema and tables: {e}")
        supabase_conn.rollback()
        return False

def migrate_table_data(local_conn, supabase_conn, table_name):
    """Migrate data for a single table"""
    try:
        local_cursor = local_conn.cursor()
        supabase_cursor = supabase_conn.cursor()
        
        logger.info(f"Migrating data for table: {table_name}")
        
        # Get data from local
        local_cursor.execute(f'SELECT * FROM public."{table_name}"')
        rows = local_cursor.fetchall()
        
        if rows:
            # Get column names
            local_cursor.execute(f"""
                SELECT column_name 
                FROM information_schema.columns 
                WHERE table_name = '{table_name}' AND table_schema = 'public'
                ORDER BY ordinal_position
            """)
            columns = [col[0] for col in local_cursor.fetchall()]
            
            # Build INSERT statement
            quoted_columns = [f'"{col}"' for col in columns]
            insert_sql = f'INSERT INTO ecole_db."{table_name}" ({", ".join(quoted_columns)}) VALUES %s'
            
            # Insert data in batches
            batch_size = 1000
            for i in range(0, len(rows), batch_size):
                batch = rows[i:i + batch_size]
                execute_values(supabase_cursor, insert_sql, batch)
                supabase_conn.commit()
                
                if i % 10000 == 0 or i + batch_size >= len(rows):
                    logger.info(f"  Inserted {min(i + batch_size, len(rows))}/{len(rows)} records")
        
        local_cursor.close()
        supabase_cursor.close()
        
        logger.info(f"✅ Migrated table {table_name} with {len(rows)} records")
        return len(rows)
        
    except Exception as e:
        logger.error(f"❌ Failed to migrate table {table_name}: {e}")
        supabase_conn.rollback()
        return -1

def migrate_all_data():
    """Migrate all data from local PostgreSQL to Supabase"""
    try:
        # Connect to both databases
        local_conn = psycopg2.connect(**LOCAL_POSTGRES_CONFIG)
        supabase_conn = psycopg2.connect(**SUPABASE_CONFIG)
        supabase_conn.autocommit = False
        
        logger.info("Connected to both databases")
        
        # Get all tables
        local_cursor = local_conn.cursor()
        local_cursor.execute("""
            SELECT table_name 
            FROM information_schema.tables 
            WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
            ORDER BY table_name
        """)
        tables = [table[0] for table in local_cursor.fetchall()]
        local_cursor.close()
        
        logger.info(f"Found {len(tables)} tables to migrate")
        
        # Create schema and tables first
        if not create_schema_and_tables(supabase_conn):
            return False
        
        # Migrate data for each table
        success_count = 0
        total_records = 0
        failed_tables = []
        
        for table in tables:
            record_count = migrate_table_data(local_conn, supabase_conn, table)
            if record_count >= 0:
                success_count += 1
                total_records += record_count
            else:
                failed_tables.append(table)
        
        # Summary
        logger.info("=" * 60)
        logger.info("SUPABASE MIGRATION SUMMARY")
        logger.info("=" * 60)
        logger.info(f"Tables migrated successfully: {success_count}/{len(tables)}")
        logger.info(f"Total records migrated: {total_records}")
        
        if failed_tables:
            logger.warning(f"Failed tables ({len(failed_tables)}): {', '.join(failed_tables)}")
        
        logger.info("=" * 60)
        
        # Close connections
        local_conn.close()
        supabase_conn.close()
        
        return success_count == len(tables)
        
    except Exception as e:
        logger.error(f"Migration failed: {e}")
        return False

def main():
    logger.info("=== Starting Migration to Supabase ===")
    
    # Test Supabase connection first
    if not test_supabase_connection():
        logger.error("Cannot proceed - Supabase connection failed")
        return 1
    
    # Migrate all data
    success = migrate_all_data()
    
    if success:
        logger.info("🎉 Migration to Supabase completed successfully!")
        return 0
    else:
        logger.error("💥 Migration to Supabase failed!")
        return 1

if __name__ == "__main__":
    sys.exit(main())