DO $$
DECLARE
    _col record;
BEGIN
	CREATE SCHEMA IF NOT EXISTS public;

	CREATE TEMP TABLE IF NOT EXISTS temp_table_info_public_slip_classification AS
	SELECT column_name, data_type, is_nullable
	FROM information_schema.columns 
	WHERE table_schema = 'public' 
	AND table_name = 'slip_classification';

	CREATE TABLE IF NOT EXISTS public.slip_classification (
	                classification_id integer PRIMARY KEY NOT NULL,
	            slip_name text NOT NULL,
	            max_length numeric NOT NULL,
	            max_beam numeric NOT NULL,
	            base_price numeric NOT NULL,
	            description text NOT NULL
	    );

	ALTER TABLE public.slip_classification
	    ADD COLUMN IF NOT EXISTS classification_id integer NOT NULL;

	ALTER TABLE public.slip_classification
	    ADD COLUMN IF NOT EXISTS slip_name text NOT NULL;

	ALTER TABLE public.slip_classification
	    ADD COLUMN IF NOT EXISTS max_length numeric NOT NULL;

	ALTER TABLE public.slip_classification
	    ADD COLUMN IF NOT EXISTS max_beam numeric NOT NULL;

	ALTER TABLE public.slip_classification
	    ADD COLUMN IF NOT EXISTS base_price numeric NOT NULL;

	ALTER TABLE public.slip_classification
	    ADD COLUMN IF NOT EXISTS description text NOT NULL;

	IF EXISTS (
	    SELECT 1 FROM temp_table_info_public_slip_classification
	    WHERE column_name = 'classification_id'
	    AND (
	        data_type != 'integer'
	        OR is_nullable != 'NO'
	    )
	) THEN
	    ALTER TABLE public.slip_classification
	    ALTER COLUMN classification_id TYPE integer USING classification_id::integer,
	    ALTER COLUMN classification_id SET NOT NULL;
	END IF;

	IF EXISTS (
	    SELECT 1 FROM temp_table_info_public_slip_classification
	    WHERE column_name = 'slip_name'
	    AND (
	        data_type != 'text'
	        OR is_nullable != 'NO'
	    )
	) THEN
	    ALTER TABLE public.slip_classification
	    ALTER COLUMN slip_name TYPE text USING slip_name::text,
	    ALTER COLUMN slip_name SET NOT NULL;
	END IF;

	IF EXISTS (
	    SELECT 1 FROM temp_table_info_public_slip_classification
	    WHERE column_name = 'max_length'
	    AND (
	        data_type != 'numeric'
	        OR is_nullable != 'NO'
	    )
	) THEN
	    ALTER TABLE public.slip_classification
	    ALTER COLUMN max_length TYPE numeric USING max_length::numeric,
	    ALTER COLUMN max_length SET NOT NULL;
	END IF;

	IF EXISTS (
	    SELECT 1 FROM temp_table_info_public_slip_classification
	    WHERE column_name = 'max_beam'
	    AND (
	        data_type != 'numeric'
	        OR is_nullable != 'NO'
	    )
	) THEN
	    ALTER TABLE public.slip_classification
	    ALTER COLUMN max_beam TYPE numeric USING max_beam::numeric,
	    ALTER COLUMN max_beam SET NOT NULL;
	END IF;

	IF EXISTS (
	    SELECT 1 FROM temp_table_info_public_slip_classification
	    WHERE column_name = 'base_price'
	    AND (
	        data_type != 'numeric'
	        OR is_nullable != 'NO'
	    )
	) THEN
	    ALTER TABLE public.slip_classification
	    ALTER COLUMN base_price TYPE numeric USING base_price::numeric,
	    ALTER COLUMN base_price SET NOT NULL;
	END IF;

	IF EXISTS (
	    SELECT 1 FROM temp_table_info_public_slip_classification
	    WHERE column_name = 'description'
	    AND (
	        data_type != 'text'
	        OR is_nullable != 'NO'
	    )
	) THEN
	    ALTER TABLE public.slip_classification
	    ALTER COLUMN description TYPE text USING description::text,
	    ALTER COLUMN description SET NOT NULL;
	END IF;

	FOR _col IN (
	    SELECT column_name FROM temp_table_info_public_slip_classification
	    WHERE column_name NOT IN ('classification_id', 'slip_name', 'max_length', 'max_beam', 'base_price', 'description')
	)
	LOOP
	    EXECUTE format('ALTER TABLE %I.%I DROP COLUMN %I', 'public', 'slip_classification', _col.column_name);
	END LOOP;


	DROP TABLE IF EXISTS temp_table_info_public_slip_classification;
END $$;
